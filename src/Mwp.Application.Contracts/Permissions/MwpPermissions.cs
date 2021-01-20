namespace Mwp.Permissions
{
    public static class MwpPermissions
    {
        public const string GroupName = "Mwp";

        public static class Dashboard
        {
            public const string DashboardGroup = GroupName + ".Dashboard";
            public const string Host = DashboardGroup + ".Host";
            public const string Tenant = GroupName + ".Tenant";
        }

        //Add your own permission names here.

        public static class File
        {
            public const string Default = GroupName + ".File";
        }

        public static class Form
        {
            public const string Default = GroupName + ".Form";
        }

        public class Wopi
        {
            public const string Default = GroupName + ".Wopi";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
            public const string PutRelative = Default + ".PutRelative";
            public const string Rename = Default + ".Rename";
            public const string WopiTest = Default + ".WopiTest";
            public const string EmbedView = Default + ".EmbedView";
            public const string EnhancedView = Default + ".EnhancedView";
        }

        public class Xero
        {
            public const string Default = GroupName + ".Xero";
            public const string Accounting = Default + ".Accounting";
        }

        public class Qbo
        {
            public const string Default = GroupName + ".Qbo";
            public const string Accounting = Default + ".Accounting";
        }
        
        public class Pdf
        {
            public const string Default = GroupName + ".Pdf";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
            public const string EmbedView = Default + ".EmbedView";
        }

        public class Accounts
        {
            public const string Default = GroupName + ".Accounts";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Addresses
        {
            public const string Default = GroupName + ".Addresses";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Authors
        {
            public const string Default = GroupName + ".Authors";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Communications
        {
            public const string Default = GroupName + ".Communications";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Components
        {
            public const string Default = GroupName + ".Components";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Documents
        {
            public const string Default = GroupName + ".Documents";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Emails
        {
            public const string Default = GroupName + ".Emails";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Engagements
        {
            public const string Default = GroupName + ".Engagements";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Entities
        {
            public const string Default = GroupName + ".Entities";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class EntityGroups
        {
            public const string Default = GroupName + ".EntityGroups";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class EntityTypes
        {
            public const string Default = GroupName + ".EntityTypes";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Folders
        {
            public const string Default = GroupName + ".Folders";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Ledgers
        {
            public const string Default = GroupName + ".Ledgers";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Phones
        {
            public const string Default = GroupName + ".Phones";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Publishers
        {
            public const string Default = GroupName + ".Publishers";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class RelationshipTypes
        {
            public const string Default = GroupName + ".RelationshipTypes";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Templates
        {
            public const string Default = GroupName + ".Templates";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class TenantExes
        {
            public const string Default = GroupName + ".TenantExes";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class TenantTypes
        {
            public const string Default = GroupName + ".TenantTypes";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Titles
        {
            public const string Default = GroupName + ".Titles";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class TitleCategories
        {
            public const string Default = GroupName + ".TitleCategories";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Websites
        {
            public const string Default = GroupName + ".Websites";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Workbooks
        {
            public const string Default = GroupName + ".Workbooks";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public class Workpapers
        {
            public const string Default = GroupName + ".Workpapers";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
    }
}