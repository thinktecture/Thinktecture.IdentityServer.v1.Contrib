using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Profile;

using Microsoft.IdentityModel.Claims;
using Thinktecture.IdentityServer.TokenService;
using Thinktecture.IdentityServer.Repositories;

using ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Configuration;

namespace ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Repositories
{
    public class ActiveDirectoryProviderUserRepository : IUserRepository
    {
        private const string ProfileClaimPrefix = "http://identityserver.thinktecture.com/claims/profileclaims/";

        [Import]
        public IClientCertificatesRepository Repository { get; set; }

        public ActiveDirectoryProviderUserRepository()
        {
            Container.Current.SatisfyImportsOnce(this);
        }

        public bool ValidateUser(string userName, string password)
        {
            using (PrincipalContext context = GetPrincipalContext())
            {
                return context.ValidateCredentials(userName, password);
            }
        }

        public bool ValidateUser(X509Certificate2 clientCertificate, out string userName)
        {
            return Repository.TryGetUserNameFromThumbprint(clientCertificate, out userName);
        }

        public IEnumerable<string> GetRoles(string userName, RoleTypes roleType)
        {
            var returnedRoles = new List<string>();

            using (PrincipalContext context = GetPrincipalContext())
            {
                // find the user in the identity store
                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);

                // get the groups for the user principal and
                // store the results in a PrincipalSearchResult object
                PrincipalSearchResult<Principal> results = user.GetGroups();

                results.ToList().ForEach(result => returnedRoles.Add(result.Name));
            }
            return returnedRoles;
        }

        public IEnumerable<Microsoft.IdentityModel.Claims.Claim> GetClaims(IClaimsPrincipal principal, RequestDetails requestDetails)
        {
            var claims = new List<Claim>();

            using (PrincipalContext context = GetPrincipalContext())
            {
                var userName = principal.Identity.Name;

                // find the user in the identity store
                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);

                // email address
                string email = user.EmailAddress;
                if (!String.IsNullOrEmpty(email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, email));
                }

                // roles
                GetRoles(userName, RoleTypes.Client).ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

                // profile claims
                if (ProfileManager.Enabled)
                {
                    var profile = ProfileBase.Create(userName, true);
                    if (profile != null)
                    {
                        foreach (SettingsProperty prop in ProfileBase.Properties)
                        {
                            string value = profile.GetPropertyValue(prop.Name).ToString();
                            if (!String.IsNullOrWhiteSpace(value))
                            {
                                claims.Add(new Claim(ProfileClaimPrefix + prop.Name.ToLowerInvariant(), value));
                            }
                        }
                    }
                }
            }

            return claims;
        }

        public IEnumerable<string> GetSupportedClaimTypes()
        {
            var claimTypes = new List<string>
            {
                ClaimTypes.Name,
                ClaimTypes.Email,
                ClaimTypes.Role
            };

            if (ProfileManager.Enabled)
            {
                foreach (SettingsProperty prop in ProfileBase.Properties)
                {
                    claimTypes.Add(ProfileClaimPrefix + prop.Name.ToLowerInvariant());
                }
            }

            return claimTypes;
        }

        private PrincipalContext GetPrincipalContext()
        {
            var section = ConfigurationManager.GetSection(ActiveDirectoryConfigurationSection.SectionName) as ActiveDirectoryConfigurationSection;

            string container = string.IsNullOrEmpty(section.Container) ? null : section.Container;
            string userName = string.IsNullOrEmpty(section.UserName) ? null : section.UserName;
            string password = string.IsNullOrEmpty(section.Password) ? null : section.Password;

            return new PrincipalContext(section.ContextType, section.DomainName, container, userName, password);
        }
    }
}
