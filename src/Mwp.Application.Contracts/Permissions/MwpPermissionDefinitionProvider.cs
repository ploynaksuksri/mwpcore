using Mwp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Saas.Host;

namespace Mwp.Permissions
{
    public class MwpPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(MwpPermissions.GroupName);

            myGroup.AddPermission(MwpPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
            myGroup.AddPermission(MwpPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(MwpPermissions.MyPermission1, L("Permission:MyPermission1"));

            myGroup.AddPermission(MwpPermissions.File.Default, L("Permission:File"));

            myGroup.AddPermission(MwpPermissions.Form.Default, L("Permission:Form"));

            var wopiPermission = myGroup.AddPermission(MwpPermissions.Wopi.Default, L("Permission:Wopi"));
            wopiPermission.AddChild(MwpPermissions.Wopi.Edit, L("Permission:Edit"));
            wopiPermission.AddChild(MwpPermissions.Wopi.Delete, L("Permission:Delete"));
            wopiPermission.AddChild(MwpPermissions.Wopi.PutRelative, L("Permission:PutRelative"));
            wopiPermission.AddChild(MwpPermissions.Wopi.Rename, L("Permission:Rename"));
            wopiPermission.AddChild(MwpPermissions.Wopi.WopiTest, L("Permission:WopiTest"));
            wopiPermission.AddChild(MwpPermissions.Wopi.EmbedView, L("Permission:EmbedView"));
            wopiPermission.AddChild(MwpPermissions.Wopi.EnhancedView, L("Permission:EnhancedView"));

            myGroup.AddPermission(MwpPermissions.Xero.Default, L("Permission:Xero"));
            myGroup.AddPermission(MwpPermissions.Qbo.Default, L("Permission:Qbo"));

            var pdfPermission = myGroup.AddPermission(MwpPermissions.Pdf.Default, L("Permission:Pdf"));
            pdfPermission.AddChild(MwpPermissions.Pdf.Edit, L("Permission:Edit"));
            pdfPermission.AddChild(MwpPermissions.Pdf.Delete, L("Permission:Delete"));
            pdfPermission.AddChild(MwpPermissions.Pdf.EmbedView, L("Permission:EmbedView"));

            //Account
            var accountPermission = myGroup.AddPermission(MwpPermissions.Accounts.Default, L("Permission:Accounts"));
            accountPermission.AddChild(MwpPermissions.Accounts.Create, L("Permission:Create"));
            accountPermission.AddChild(MwpPermissions.Accounts.Edit, L("Permission:Edit"));
            accountPermission.AddChild(MwpPermissions.Accounts.Delete, L("Permission:Delete"));

            //Address
            var addressPermission = myGroup.AddPermission(MwpPermissions.Addresses.Default, L("Permission:Addresses"));
            addressPermission.AddChild(MwpPermissions.Addresses.Create, L("Permission:Create"));
            addressPermission.AddChild(MwpPermissions.Addresses.Edit, L("Permission:Edit"));
            addressPermission.AddChild(MwpPermissions.Addresses.Delete, L("Permission:Delete"));

            //Author
            var authorPermission = myGroup.AddPermission(MwpPermissions.Authors.Default, L("Permission:Authors"));
            authorPermission.AddChild(MwpPermissions.Authors.Create, L("Permission:Create"));
            authorPermission.AddChild(MwpPermissions.Authors.Edit, L("Permission:Edit"));
            authorPermission.AddChild(MwpPermissions.Authors.Delete, L("Permission:Delete"));

            //Communication
            var communicationPermission = myGroup.AddPermission(MwpPermissions.Communications.Default, L("Permission:Communications"));
            communicationPermission.AddChild(MwpPermissions.Communications.Create, L("Permission:Create"));
            communicationPermission.AddChild(MwpPermissions.Communications.Edit, L("Permission:Edit"));
            communicationPermission.AddChild(MwpPermissions.Communications.Delete, L("Permission:Delete"));

            //Component
            var componentPermission = myGroup.AddPermission(MwpPermissions.Components.Default, L("Permission:Components"));
            componentPermission.AddChild(MwpPermissions.Components.Create, L("Permission:Create"));
            componentPermission.AddChild(MwpPermissions.Components.Edit, L("Permission:Edit"));
            componentPermission.AddChild(MwpPermissions.Components.Delete, L("Permission:Delete"));

            //Document
            var documentPermission = myGroup.AddPermission(MwpPermissions.Documents.Default, L("Permission:Documents"));
            documentPermission.AddChild(MwpPermissions.Documents.Create, L("Permission:Create"));
            documentPermission.AddChild(MwpPermissions.Documents.Edit, L("Permission:Edit"));
            documentPermission.AddChild(MwpPermissions.Documents.Delete, L("Permission:Delete"));

            //Email
            var emailPermission = myGroup.AddPermission(MwpPermissions.Emails.Default, L("Permission:Emails"));
            emailPermission.AddChild(MwpPermissions.Emails.Create, L("Permission:Create"));
            emailPermission.AddChild(MwpPermissions.Emails.Edit, L("Permission:Edit"));
            emailPermission.AddChild(MwpPermissions.Emails.Delete, L("Permission:Delete"));

            //Engagement
            var engagementPermission = myGroup.AddPermission(MwpPermissions.Engagements.Default, L("Permission:Engagements"));
            engagementPermission.AddChild(MwpPermissions.Engagements.Create, L("Permission:Create"));
            engagementPermission.AddChild(MwpPermissions.Engagements.Edit, L("Permission:Edit"));
            engagementPermission.AddChild(MwpPermissions.Engagements.Delete, L("Permission:Delete"));

            //Entity
            var entityPermission = myGroup.AddPermission(MwpPermissions.Entities.Default, L("Permission:Entities"));
            entityPermission.AddChild(MwpPermissions.Entities.Create, L("Permission:Create"));
            entityPermission.AddChild(MwpPermissions.Entities.Edit, L("Permission:Edit"));
            entityPermission.AddChild(MwpPermissions.Entities.Delete, L("Permission:Delete"));

            //EntityGroup
            var entityGroupPermission = myGroup.AddPermission(MwpPermissions.EntityGroups.Default, L("Permission:EntityGroups"));
            entityGroupPermission.AddChild(MwpPermissions.EntityGroups.Create, L("Permission:Create"));
            entityGroupPermission.AddChild(MwpPermissions.EntityGroups.Edit, L("Permission:Edit"));
            entityGroupPermission.AddChild(MwpPermissions.EntityGroups.Delete, L("Permission:Delete"));

            //EntityType
            var entityTypePermission = myGroup.AddPermission(MwpPermissions.EntityTypes.Default, L("Permission:EntityTypes"));
            entityTypePermission.AddChild(MwpPermissions.EntityTypes.Create, L("Permission:Create"));
            entityTypePermission.AddChild(MwpPermissions.EntityTypes.Edit, L("Permission:Edit"));
            entityTypePermission.AddChild(MwpPermissions.EntityTypes.Delete, L("Permission:Delete"));

            //Folder
            var folderPermission = myGroup.AddPermission(MwpPermissions.Folders.Default, L("Permission:Folders"));
            folderPermission.AddChild(MwpPermissions.Folders.Create, L("Permission:Create"));
            folderPermission.AddChild(MwpPermissions.Folders.Edit, L("Permission:Edit"));
            folderPermission.AddChild(MwpPermissions.Folders.Delete, L("Permission:Delete"));

            //Ledger
            var ledgerPermission = myGroup.AddPermission(MwpPermissions.Ledgers.Default, L("Permission:Ledgers"));
            ledgerPermission.AddChild(MwpPermissions.Ledgers.Create, L("Permission:Create"));
            ledgerPermission.AddChild(MwpPermissions.Ledgers.Edit, L("Permission:Edit"));
            ledgerPermission.AddChild(MwpPermissions.Ledgers.Delete, L("Permission:Delete"));

            //Phone
            var phonePermission = myGroup.AddPermission(MwpPermissions.Phones.Default, L("Permission:Phones"));
            phonePermission.AddChild(MwpPermissions.Phones.Create, L("Permission:Create"));
            phonePermission.AddChild(MwpPermissions.Phones.Edit, L("Permission:Edit"));
            phonePermission.AddChild(MwpPermissions.Phones.Delete, L("Permission:Delete"));

            //Publisher
            var publisherPermission = myGroup.AddPermission(MwpPermissions.Publishers.Default, L("Permission:Publishers"));
            publisherPermission.AddChild(MwpPermissions.Publishers.Create, L("Permission:Create"));
            publisherPermission.AddChild(MwpPermissions.Publishers.Edit, L("Permission:Edit"));
            publisherPermission.AddChild(MwpPermissions.Publishers.Delete, L("Permission:Delete"));

            //RelationshipType
            var relationshipTypePermission = myGroup.AddPermission(MwpPermissions.RelationshipTypes.Default, L("Permission:RelationshipTypes"));
            relationshipTypePermission.AddChild(MwpPermissions.RelationshipTypes.Create, L("Permission:Create"));
            relationshipTypePermission.AddChild(MwpPermissions.RelationshipTypes.Edit, L("Permission:Edit"));
            relationshipTypePermission.AddChild(MwpPermissions.RelationshipTypes.Delete, L("Permission:Delete"));

            //Template
            var templatePermission = myGroup.AddPermission(MwpPermissions.Templates.Default, L("Permission:Templates"));
            templatePermission.AddChild(MwpPermissions.Templates.Create, L("Permission:Create"));
            templatePermission.AddChild(MwpPermissions.Templates.Edit, L("Permission:Edit"));
            templatePermission.AddChild(MwpPermissions.Templates.Delete, L("Permission:Delete"));

            //TenantEx
            var tenantExPermission = myGroup.AddPermission(MwpPermissions.TenantExes.Default, L("Permission:TenantExes"));
            tenantExPermission.AddChild(MwpPermissions.TenantExes.Create, L("Permission:Create"));
            tenantExPermission.AddChild(MwpPermissions.TenantExes.Edit, L("Permission:Edit"));
            tenantExPermission.AddChild(MwpPermissions.TenantExes.Delete, L("Permission:Delete"));

            //TenantType
            var tenantTypePermission = myGroup.AddPermission(MwpPermissions.TenantTypes.Default, L("Permission:TenantTypes"));
            tenantTypePermission.AddChild(MwpPermissions.TenantTypes.Create, L("Permission:Create"));
            tenantTypePermission.AddChild(MwpPermissions.TenantTypes.Edit, L("Permission:Edit"));
            tenantTypePermission.AddChild(MwpPermissions.TenantTypes.Delete, L("Permission:Delete"));

            //Title
            var titlePermission = myGroup.AddPermission(MwpPermissions.Titles.Default, L("Permission:Titles"));
            titlePermission.AddChild(MwpPermissions.Titles.Create, L("Permission:Create"));
            titlePermission.AddChild(MwpPermissions.Titles.Edit, L("Permission:Edit"));
            titlePermission.AddChild(MwpPermissions.Titles.Delete, L("Permission:Delete"));

            //TitleCategory
            var titleCategoryPermission = myGroup.AddPermission(MwpPermissions.TitleCategories.Default, L("Permission:TitleCategories"));
            titleCategoryPermission.AddChild(MwpPermissions.TitleCategories.Create, L("Permission:Create"));
            titleCategoryPermission.AddChild(MwpPermissions.TitleCategories.Edit, L("Permission:Edit"));
            titleCategoryPermission.AddChild(MwpPermissions.TitleCategories.Delete, L("Permission:Delete"));

            //Website
            var websitePermission = myGroup.AddPermission(MwpPermissions.Websites.Default, L("Permission:Websites"));
            websitePermission.AddChild(MwpPermissions.Websites.Create, L("Permission:Create"));
            websitePermission.AddChild(MwpPermissions.Websites.Edit, L("Permission:Edit"));
            websitePermission.AddChild(MwpPermissions.Websites.Delete, L("Permission:Delete"));

            //Workbook
            var workbookPermission = myGroup.AddPermission(MwpPermissions.Workbooks.Default, L("Permission:Workbooks"));
            workbookPermission.AddChild(MwpPermissions.Workbooks.Create, L("Permission:Create"));
            workbookPermission.AddChild(MwpPermissions.Workbooks.Edit, L("Permission:Edit"));
            workbookPermission.AddChild(MwpPermissions.Workbooks.Delete, L("Permission:Delete"));

            //Workpaper
            var workpaperPermission = myGroup.AddPermission(MwpPermissions.Workpapers.Default, L("Permission:Workpapers"));
            workpaperPermission.AddChild(MwpPermissions.Workpapers.Create, L("Permission:Create"));
            workpaperPermission.AddChild(MwpPermissions.Workpapers.Edit, L("Permission:Edit"));
            workpaperPermission.AddChild(MwpPermissions.Workpapers.Delete, L("Permission:Delete"));

            ReassignSaasPermissions(context);
        }

        private static void ReassignSaasPermissions(IPermissionDefinitionContext context)
        {
            context.RemoveGroup(SaasHostPermissions.GroupName);

            //Saas
            var saasGroup = context.AddGroup(SaasHostPermissions.GroupName, L("Permission:Saas"));

            //Tenant
            var tenantsPermission = saasGroup.AddPermission(SaasHostPermissions.Tenants.Default, L("Permission:TenantManagement"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.Create, L("Permission:Create"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.Update, L("Permission:Edit"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.Delete, L("Permission:Delete"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.ManageFeatures, L("Permission:ManageFeatures"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.ManageConnectionStrings, L("Permission:ManageConnectionStrings"));
            tenantsPermission.AddChild(SaasHostPermissions.Tenants.ViewChangeHistory, L("Permission:ViewChangeHistory"));

            //Edition
            var editionsPermission = saasGroup.AddPermission(SaasHostPermissions.Editions.Default, L("Permission:EditionManagement"), MultiTenancySides.Host);
            editionsPermission.AddChild(SaasHostPermissions.Editions.Create, L("Permission:Create"), MultiTenancySides.Host);
            editionsPermission.AddChild(SaasHostPermissions.Editions.Update, L("Permission:Edit"), MultiTenancySides.Host);
            editionsPermission.AddChild(SaasHostPermissions.Editions.Delete, L("Permission:Delete"), MultiTenancySides.Host);
            editionsPermission.AddChild(SaasHostPermissions.Editions.ManageFeatures, L("Permission:ManageFeatures"), MultiTenancySides.Host);
            editionsPermission.AddChild(SaasHostPermissions.Editions.ViewChangeHistory, L("Permission:ViewChangeHistory"), MultiTenancySides.Host);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MwpResource>(name);
        }
    }
}