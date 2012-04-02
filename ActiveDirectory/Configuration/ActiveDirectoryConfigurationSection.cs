using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace ThinkTecture.IdentityServer.Contrib.ActiveDirectory.Configuration
{
    public partial class ActiveDirectoryConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "thinktecture.identityserver.contrib.activedirectory";

        #region ContextType Property

        internal const String ContextTypePropertyName = "contextType";

        [ConfigurationProperty(ContextTypePropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false, DefaultValue = "Domain")]
        public ContextType ContextType
        {
            get
            {
                return (ContextType)base[ContextTypePropertyName];
            }
            set
            {
                base[ContextTypePropertyName] = value;
            }
        }

        #endregion

        #region DomainName Property

        internal const String DomainNamePropertyName = "domainName";

        [ConfigurationProperty(DomainNamePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
        public String DomainName
        {
            get
            {
                return (String)base[DomainNamePropertyName];
            }
            set
            {
                base[DomainNamePropertyName] = value;
            }
        }

        #endregion

        #region Container Property

        internal const String ContainerPropertyName = "container";

        [ConfigurationProperty(ContainerPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public String Container
        {
            get
            {
                return (String)base[ContainerPropertyName];
            }
            set
            {
                base[ContainerPropertyName] = value;
            }
        }

        #endregion

        #region UserName Property

        internal const String UserNamePropertyName = "userName";

        [ConfigurationProperty(UserNamePropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public String UserName
        {
            get
            {
                return (String)base[UserNamePropertyName];
            }
            set
            {
                base[UserNamePropertyName] = value;
            }
        }

        #endregion

        #region Password Property

        internal const String PasswordPropertyName = "password";

        [ConfigurationProperty(PasswordPropertyName, IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public String Password
        {
            get
            {
                return (String)base[PasswordPropertyName];
            }
            set
            {
                base[PasswordPropertyName] = value;
            }
        }

        #endregion

        #region AdministrationGroup Element

        internal const String AdministrationGroupElementName = "Administrationgroup";

        [ConfigurationProperty(AdministrationGroupElementName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
        public AdministrationGroupConfigElement AdministrationGroup
        {
            get
            {
                return (AdministrationGroupConfigElement)base[AdministrationGroupElementName];
            }
            set
            {
                base[AdministrationGroupElementName] = value;
            }
        }

        #endregion
        
        #region UserGroup Collection

        internal const String UserGroupsCollectionName = "Usergroups";

        [ConfigurationProperty(UserGroupsCollectionName, IsRequired = true, IsKey = false, IsDefaultCollection = false)]
        public UserGroupCollection UserGroups
        {
            get
            {
                return (UserGroupCollection)base[UserGroupsCollectionName];
            }
        }

        #endregion

    }

    public partial class AdministrationGroupConfigElement : ConfigurationElement
    {
        #region AdministrationGroupName Property

        internal const String AdministrationGroupNamePropertyName = "name";

        [ConfigurationProperty(AdministrationGroupNamePropertyName, IsRequired = true, IsKey = false, IsDefaultCollection = false, DefaultValue = "Administrators")]
        public String AdministrationGroupName
        {
            get
            {
                return (String)base[AdministrationGroupNamePropertyName];
            }
            set
            {
                base[AdministrationGroupNamePropertyName] = value;
            }
        }

        #endregion
    }

    public partial class UserGroupCollection : ConfigurationElementCollection
    {
        public UserGroupCollection()
            : base()
        {}

        public override ConfigurationElementCollectionType CollectionType { get { return ConfigurationElementCollectionType.BasicMap; } }

        protected override string ElementName { get { return "Usergroup"; } }

        protected override ConfigurationElement CreateNewElement()
        {
            return new UserGroupConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserGroupConfigElement)element).UserGroupName;
        }
    }

    public partial class UserGroupConfigElement : ConfigurationElement
    {
        public UserGroupConfigElement() {}

        public UserGroupConfigElement(string elementName)
        {
            UserGroupName = elementName;
        }

        #region UserGroupName Property

        internal const String UserGroupNamePropertyName = "name";

        [ConfigurationProperty(UserGroupNamePropertyName, IsRequired = true, IsKey = true, IsDefaultCollection = false)]
        public String UserGroupName
        {
            get
            {
                return (String)base[UserGroupNamePropertyName];
            }
            set
            {
                base[UserGroupNamePropertyName] = value;
            }
        }

        #endregion
    }
}
