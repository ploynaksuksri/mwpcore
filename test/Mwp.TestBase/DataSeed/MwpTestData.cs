using System;
using Volo.Abp.DependencyInjection;

namespace Mwp.DataSeed
{
    public class MwpTestData : ISingletonDependency
    {
        public Guid Country1Id { get; } = Guid.NewGuid();
        public const string Country1Name = "Turkey";

        public Guid Country2Id { get; } = Guid.NewGuid();
        public const string Country2Name = "Germany";

        public const string Country3Name = "South Africa";

        public Guid Account1Id { get; } = Guid.NewGuid();
        public Guid Account1CountryId => Country1Id;
        public Guid ParentAccount1Id { get; } = Guid.Empty;

        public Guid Account2Id { get; } = Guid.NewGuid();
        public Guid Account2CountryId => Country2Id;
        public Guid ParentAccount2Id => Account1Id;

        public Guid Contact1Id { get; } = Guid.NewGuid();
        public Guid Contact2Id { get; } = Guid.NewGuid();
        public Guid Contact3Id { get; } = Guid.NewGuid();

        public Guid Contact1AccountId => Account1Id;
        public Guid Contact2AccountId => Account1Id;
        public Guid Contact3AccountId => Account2Id;

        public Guid ProductGroup1Id { get; } = Guid.NewGuid(); // Base Group
        public Guid ProductGroup2Id { get; } = Guid.NewGuid(); // Top = 1
        public Guid ProductGroup3Id { get; } = Guid.NewGuid(); // Top = 2
        public Guid ProductGroup4Id { get; } = Guid.NewGuid(); // Base Group

        public Guid? ProductGroup1TopGroupId => null;
        public Guid? ProductGroup2TopGroupId => ProductGroup1Id;
        public Guid? ProductGroup3TopGroupId => ProductGroup2Id;
        public Guid? ProductGroup4TopGroupId => null;

        public Guid Product1Id { get; } = Guid.NewGuid();
        public Guid Product2Id { get; } = Guid.NewGuid();
        public Guid Product3Id { get; } = Guid.NewGuid();
        public Guid Product4Id { get; } = Guid.NewGuid();

        public decimal Product1Price { get; } = (decimal) 54.54;
        public decimal Product2Price { get; } = (decimal) 142.214;
        public decimal Product3Price { get; } = (decimal) 12.4;
        public decimal Product4Price { get; } = (decimal) 14.53;

        public Guid Order1Id { get; } = Guid.NewGuid();
        public Guid Order2Id { get; } = Guid.NewGuid();
        public Guid Order3Id { get; } = Guid.NewGuid();
        public Guid Order4Id { get; } = Guid.NewGuid();

        public Guid OrderLine1Id { get; } = Guid.NewGuid();
        public Guid OrderLine2Id { get; } = Guid.NewGuid();
        public Guid OrderLine3Id { get; } = Guid.NewGuid();
        public Guid OrderLine4Id { get; } = Guid.NewGuid();

        public Guid Tenant1Id { get; } = Guid.NewGuid();
        public Guid Tenant2Id { get; } = Guid.NewGuid();

        public Guid Address1Id { get; } = Guid.NewGuid();
        public const string Address1Name = "Address 1";
        public bool Address1IsActive { get; } = false;

        public Guid Address2Id { get; } = Guid.NewGuid();
        public const string Address2Name = "Address 2";
        public bool Address2IsActive { get; } = true;

        public Guid Author1Id { get; } = Guid.NewGuid();
        public const string Author1Name = "Author 1";
        public bool Author1IsActive { get; } = false;

        public Guid Author2Id { get; } = Guid.NewGuid();
        public const string Author2Name = "Author 2";
        public bool Author2IsActive { get; } = true;

        public Guid Communication1Id { get; } = Guid.NewGuid();
        public const string Communication1Name = "Communication 1";
        public bool Communication1IsActive { get; } = false;

        public Guid Communication2Id { get; } = Guid.NewGuid();
        public const string Communication2Name = "Communication 2";
        public bool Communication2IsActive { get; } = true;

        public Guid Component1Id { get; } = Guid.NewGuid();
        public const string Component1Name = "Component 1";
        public bool Component1IsActive { get; } = false;

        public Guid Component2Id { get; } = Guid.NewGuid();
        public const string Component2Name = "Component 2";
        public bool Component2IsActive { get; } = true;

        public Guid Document1Id { get; } = Guid.NewGuid();
        public const string Document1Name = "Document 1";
        public bool Document1IsActive { get; } = false;

        public Guid Document2Id { get; } = Guid.NewGuid();
        public const string Document2Name = "Document 2";
        public bool Document2IsActive { get; } = true;

        public Guid Email1Id { get; } = Guid.NewGuid();
        public const string Email1Name = "Email 1";
        public bool Email1IsActive { get; } = false;

        public Guid Email2Id { get; } = Guid.NewGuid();
        public const string Email2Name = "Email 2";
        public bool Email2IsActive { get; } = true;

        public Guid Engagement1Id { get; } = Guid.NewGuid();
        public const string Engagement1Name = "Engagement 1";
        public bool Engagement1IsActive { get; } = false;

        public Guid Engagement2Id { get; } = Guid.NewGuid();
        public const string Engagement2Name = "Engagement 2";
        public bool Engagement2IsActive { get; } = true;

        public Guid Entity1Id { get; } = Guid.NewGuid();
        public const string Entity1Name = "Entity 1";
        public bool Entity1IsActive { get; } = false;

        public Guid Entity2Id { get; } = Guid.NewGuid();
        public const string Entity2Name = "Entity 2";
        public bool Entity2IsActive { get; } = true;

        public Guid EntityGroup1Id { get; } = Guid.NewGuid();
        public const string EntityGroup1Name = "EntityGroup 1";
        public bool EntityGroup1IsActive { get; } = false;

        public Guid EntityGroup2Id { get; } = Guid.NewGuid();
        public const string EntityGroup2Name = "EntityGroup 2";
        public bool EntityGroup2IsActive { get; } = true;

        public Guid EntityType1Id { get; } = Guid.NewGuid();
        public const string EntityType1Name = "EntityType 1";
        public bool EntityType1IsActive { get; } = false;

        public Guid EntityType2Id { get; } = Guid.NewGuid();
        public const string EntityType2Name = "EntityType 2";
        public bool EntityType2IsActive { get; } = true;

        public Guid Folder1Id { get; } = Guid.NewGuid();
        public const string Folder1Name = "Folder 1";
        public bool Folder1IsActive { get; } = false;
        public Guid ParentFolder1Id { get; } = Guid.Empty;

        public Guid Folder2Id { get; } = Guid.NewGuid();
        public const string Folder2Name = "Folder 2";
        public bool Folder2IsActive { get; } = true;
        public Guid ParentFolder2Id => Folder1Id;

        public Guid Ledger1Id { get; } = Guid.NewGuid();
        public const string Ledger1Name = "Ledger 1";
        public bool Ledger1IsActive { get; } = false;

        public Guid Ledger2Id { get; } = Guid.NewGuid();
        public const string Ledger2Name = "Ledger 2";
        public bool Ledger2IsActive { get; } = true;

        public Guid Phone1Id { get; } = Guid.NewGuid();
        public const string Phone1Name = "Phone 1";
        public bool Phone1IsActive { get; } = false;

        public Guid Phone2Id { get; } = Guid.NewGuid();
        public const string Phone2Name = "Phone 2";
        public bool Phone2IsActive { get; } = true;

        public Guid Publisher1Id { get; } = Guid.NewGuid();
        public const string Publisher1Name = "Publisher 1";
        public bool Publisher1IsActive { get; } = false;

        public Guid Publisher2Id { get; } = Guid.NewGuid();
        public const string Publisher2Name = "Publisher 2";
        public bool Publisher2IsActive { get; } = true;

        public Guid RelationshipType1Id { get; } = Guid.NewGuid();
        public const string RelationshipType1Name = "RelationshipType 1";
        public bool RelationshipType1IsActive { get; } = false;

        public Guid RelationshipType2Id { get; } = Guid.NewGuid();
        public const string RelationshipType2Name = "RelationshipType 2";
        public bool RelationshipType2IsActive { get; } = true;

        public Guid Template1Id { get; } = Guid.NewGuid();
        public const string Template1Name = "Template 1";
        public bool Template1IsActive { get; } = false;

        public Guid Template2Id { get; } = Guid.NewGuid();
        public const string Template2Name = "Template 2";
        public bool Template2IsActive { get; } = true;

        public Guid TenantEx1Id { get; } = Guid.NewGuid();
        public const string TenantEx1Name = "TenantEx 1";
        public bool TenantEx1IsActive { get; } = false;

        public Guid TenantEx2Id { get; } = Guid.NewGuid();
        public const string TenantEx2Name = "TenantEx 2";
        public bool TenantEx2IsActive { get; } = true;

        public Guid TenantType1Id { get; } = Guid.NewGuid();
        public const string TenantType1Name = "TenantType 1";
        public bool TenantType1IsActive { get; } = false;

        public Guid TenantType2Id { get; } = Guid.NewGuid();
        public const string TenantType2Name = "TenantType 2";
        public bool TenantType2IsActive { get; } = true;

        public Guid Title1Id { get; } = Guid.NewGuid();
        public const string Title1Name = "Title 1";
        public bool Title1IsActive { get; } = false;

        public Guid Title2Id { get; } = Guid.NewGuid();
        public const string Title2Name = "Title 2";
        public bool Title2IsActive { get; } = true;

        public Guid TitleCategory1Id { get; } = Guid.NewGuid();
        public const string TitleCategory1Name = "TitleCategory 1";
        public bool TitleCategory1IsActive { get; } = false;

        public Guid TitleCategory2Id { get; } = Guid.NewGuid();
        public const string TitleCategory2Name = "TitleCategory 2";
        public bool TitleCategory2IsActive { get; } = true;

        public Guid Website1Id { get; } = Guid.NewGuid();
        public const string Website1Name = "Website 1";
        public bool Website1IsActive { get; } = false;

        public Guid Website2Id { get; } = Guid.NewGuid();
        public const string Website2Name = "Website 2";
        public bool Website2IsActive { get; } = true;

        public Guid Workbook1Id { get; } = Guid.NewGuid();
        public const string Workbook1Name = "Workbook 1";
        public bool Workbook1IsActive { get; } = false;

        public Guid Workbook2Id { get; } = Guid.NewGuid();
        public const string Workbook2Name = "Workbook 2";
        public bool Workbook2IsActive { get; } = true;

        public Guid Workpaper1Id { get; } = Guid.NewGuid();
        public const string Workpaper1Name = "Workpaper 1";
        public bool Workpaper1IsActive { get; } = false;

        public Guid Workpaper2Id { get; } = Guid.NewGuid();
        public const string Workpaper2Name = "Workpaper 2";
        public bool Workpaper2IsActive { get; } = true;
    }
}