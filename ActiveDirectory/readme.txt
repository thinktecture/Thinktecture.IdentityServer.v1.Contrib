Thinktecture.IdentityServer.Contrib.ActiveDirectory is an add-on for Thinktecture IdentityServer

It implements the IdentityServer IUserRepository for validating user against a MS Active Directory (AD) or ApplictionDirectory (ADAM)

This can be accomplished by editing the web.config of IdentityServers Website
- in the <configSections> element add the following element
    <section name="thinktecture.identityserver.contrib.activedirectory" type="ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Configuration.ActiveDirectoryConfigurationSection, ThinkTecture.IdentityServer.Contrib.ActiveDirectory" />
- add a section an point to an external config file
	 <thinktecture.identityserver.contrib.activedirectory configSource="configuration\activedirectory.config" />

The external configfile called activedirectory.config should look like this
	<thinktecture.identityserver.contrib.activedirectory domainName="myDomain.local"> <!--contextType ="Machine | Domain | ApplicationDirectory" container="" userName="Administrator" password="Pass0rd" --> 
	  <Administrationgroup name="Thinktecture IdentityServer Administrators" />
	  <Usergroups>
		<Usergroup name="Domain Users" />
	  </Usergroups>
	</thinktecture.identityserver.contrib.activedirectory>
	
The properties for the activedirectory element are used to get a System.DirectoryServices.AccountManagement.PrincipelContect
The members of the Administrationgroup in the config can manage the IdentyServers Website
The usergroups membership is checked when the IdentityServer is configured to enforce UsergroupMembership, so tokens wil only be authorized when the user is member of one of the groups mentioned.

To let WIF user the AD AuthorizationManager provided in this project we need to tell WIF to user ours
This can be done by setting the type of the claimsAuthorizationManager in the IdentityServer's wif.config like this 
	<claimsAuthorizationManager type="ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Security.ActiveDirectoryAuthorizationManager" />

This code is contributed by Marcel Scherpenisse.
Marcel is a senior software engineer at UNIT4 (www.unit4.com)
