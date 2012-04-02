using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;

using Microsoft.IdentityModel.Claims;

using Thinktecture.IdentityModel.Extensions;

using Thinktecture.IdentityServer;
using Thinktecture.IdentityServer.Repositories;
using Thinktecture.IdentityServer.Web.Security;
using ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Configuration;

namespace ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Security
{
    public class ActiveDirectoryAuthorizationManager : AuthorizationManager
    {
        public ActiveDirectoryAuthorizationManager()
            : base()
        {}

        public ActiveDirectoryAuthorizationManager(IConfigurationRepository configuration)
            : base(configuration)
        {}

        protected override bool AuthorizeTokenIssuance(Collection<Claim> resource, IClaimsIdentity id)
        {
            if (!ConfigurationRepository.Configuration.EnforceUsersGroupMembership)
            {
                var authResult = id.IsAuthenticated;
                if (!authResult)
                {
                    Tracing.Error("Authorization for token issuance failed because the user is anonymous");
                }

                return authResult;
            }

            var section = ConfigurationManager.GetSection(ActiveDirectoryConfigurationSection.SectionName) as ActiveDirectoryConfigurationSection;
            var roleResult = false;
            foreach (UserGroupConfigElement group in section.UserGroups)
            {
                roleResult = id.ClaimExists(ClaimTypes.Role, group.UserGroupName);
                if (roleResult)
                    break;
            }
            return roleResult;
        }

        protected override bool AuthorizeAdministration(Collection<Claim> resource, IClaimsIdentity id)
        {
            var section = ConfigurationManager.GetSection(ActiveDirectoryConfigurationSection.SectionName) as ActiveDirectoryConfigurationSection;

            var roleResult = id.ClaimExists(ClaimTypes.Role, section.AdministrationGroup.AdministrationGroupName);

            if (!roleResult)
            {
                if (resource[0].Value != Constants.Resources.UI)
                {
                    Tracing.Error(string.Format("Administration authorization failed because user {0} is not in the {1} role", id.Name, section.AdministrationGroup.AdministrationGroupName));
                }
            }

            return roleResult;
        }
    }
}
