using AutoMapper;
using Mwp.Communications;
using Mwp.Content;
using Mwp.Engagements;
using Mwp.Entities;
using Mwp.Financials;
using Mwp.Financials.Reports;
using Mwp.PdfTron;
using Mwp.Qbo;
using Mwp.Qbo.Dtos;
using Mwp.Tenants;
using Mwp.Tenants.Dtos;
using Mwp.Wopi;
using Mwp.Xero;
using Mwp.Xero.Dtos;
using Volo.Saas.Host.Dtos;
using Volo.Saas.Tenants;

namespace Mwp
{
    public class MwpApplicationAutoMapperProfile : Profile
    {
        public MwpApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            //Tenant
            CreateMap<SaasTenantDto, MwpSaasTenantDto>();
            CreateMap<Tenant, MwpSaasTenantDto>();
            CreateMap<TenantEx, MwpSaasTenantDto>()
                .ForMember(d => d.Id, m => m.MapFrom(s => s.TenantId))
                .ForMember(d => d.Name, m => m.MapFrom(s => s.Tenant.Name))
                .ForMember(d => d.EditionId, m => m.MapFrom(s => s.Tenant.EditionId));

            //WOPI
            CreateMap<WopiFile, WopiFileDto>()
                .ForMember(d => d.OwnerId, m => m.MapFrom(s => s.CreatorId.ToString()));
            CreateMap<WopiFileHistory, WopiFileHistoryDto>();

            //XERO
            CreateMap<XeroTenant, XeroTenantDto>();
            CreateMap<XeroToken, XeroTokenDto>();
            CreateMap<XeroTokenDto, XeroToken>();

            //QBO
            CreateMap<QboTenant, QboTenantDto>();
            CreateMap<QboToken, QboTokenDto>();

            //TrialBalance
            CreateMap<TrialBalanceReport, TrialBalanceReportDto>();
            CreateMap<TrialBalanceRecord, TrialBalanceRecordDto>();

            //Account
            CreateMap<AccountCreateDto, Account>();
            CreateMap<Account, AccountDto>();
            CreateMap<Account, AccountExtendedDto>();
            CreateMap<Account, AccountNameIdDto>();
            CreateMap<AccountUpdateDto, Account>();

            //Address
            CreateMap<AddressCreateDto, Address>();
            CreateMap<AddressUpdateDto, Address>();
            CreateMap<Address, AddressDto>();

            //Author
            CreateMap<AuthorCreateDto, Author>();
            CreateMap<AuthorUpdateDto, Author>();
            CreateMap<Author, AuthorDto>();

            //Communication
            CreateMap<CommunicationCreateDto, Communication>();
            CreateMap<CommunicationUpdateDto, Communication>();
            CreateMap<Communication, CommunicationDto>();

            //Component
            CreateMap<ComponentCreateDto, Component>();
            CreateMap<ComponentUpdateDto, Component>();
            CreateMap<Component, ComponentDto>();

            //Document
            CreateMap<DocumentCreateDto, Document>();
            CreateMap<DocumentUpdateDto, Document>();
            CreateMap<Document, DocumentDto>();

            //Email
            CreateMap<EmailCreateDto, Email>();
            CreateMap<EmailUpdateDto, Email>();
            CreateMap<Email, EmailDto>();

            //Engagement
            CreateMap<EngagementCreateDto, Engagement>();
            CreateMap<EngagementUpdateDto, Engagement>();
            CreateMap<Engagement, EngagementDto>();

            //Entity
            CreateMap<EntityCreateDto, Entity>();
            CreateMap<EntityUpdateDto, Entity>();
            CreateMap<Entity, EntityDto>();

            //EntityGroup
            CreateMap<EntityGroupCreateDto, EntityGroup>();
            CreateMap<EntityGroupUpdateDto, EntityGroup>();
            CreateMap<EntityGroup, EntityGroupDto>();

            //EntityType
            CreateMap<EntityTypeCreateDto, EntityType>();
            CreateMap<EntityTypeUpdateDto, EntityType>();
            CreateMap<EntityType, EntityTypeDto>();

            //Folder
            CreateMap<FolderCreateDto, Folder>();
            CreateMap<FolderUpdateDto, Folder>();
            CreateMap<Folder, FolderDto>();

            //Ledger
            CreateMap<LedgerCreateDto, Ledger>();
            CreateMap<LedgerUpdateDto, Ledger>();
            CreateMap<Ledger, LedgerDto>();

            //Phone
            CreateMap<PhoneCreateDto, Phone>();
            CreateMap<PhoneUpdateDto, Phone>();
            CreateMap<Phone, PhoneDto>();

            //Publisher
            CreateMap<PublisherCreateDto, Publisher>();
            CreateMap<PublisherUpdateDto, Publisher>();
            CreateMap<Publisher, PublisherDto>();

            //RelationshipType
            CreateMap<RelationshipTypeCreateDto, RelationshipType>();
            CreateMap<RelationshipTypeUpdateDto, RelationshipType>();
            CreateMap<RelationshipType, RelationshipTypeDto>();

            //Template
            CreateMap<TemplateCreateDto, Template>();
            CreateMap<TemplateUpdateDto, Template>();
            CreateMap<Template, TemplateDto>();

            //TenantType
            CreateMap<TenantTypeCreateDto, TenantType>();
            CreateMap<TenantTypeUpdateDto, TenantType>();
            CreateMap<TenantType, TenantTypeDto>();

            //Title
            CreateMap<TitleCreateDto, Title>();
            CreateMap<TitleUpdateDto, Title>();
            CreateMap<Title, TitleDto>();

            //TitleCategory
            CreateMap<TitleCategoryCreateDto, TitleCategory>();
            CreateMap<TitleCategoryUpdateDto, TitleCategory>();
            CreateMap<TitleCategory, TitleCategoryDto>();

            //Website
            CreateMap<WebsiteCreateDto, Website>();
            CreateMap<WebsiteUpdateDto, Website>();
            CreateMap<Website, WebsiteDto>();

            //Workbook
            CreateMap<WorkbookCreateDto, Workbook>();
            CreateMap<WorkbookUpdateDto, Workbook>();
            CreateMap<Workbook, WorkbookDto>();

            //Workpaper
            CreateMap<WorkpaperCreateDto, Workpaper>();
            CreateMap<WorkpaperUpdateDto, Workpaper>();
            CreateMap<Workpaper, WorkpaperDto>();

            //PdfTron
            CreateMap<PdfAnnotation, PdfAnnotationDto>()
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.CreatorId))
                .ForMember(d => d.CreatedDateTimeUTC, m => m.MapFrom(s => s.CreationTime));   
            CreateMap<PdfAnnotationDto, PdfAnnotation>()
                .ForMember(d => d.CreatorId, m => m.MapFrom(s => s.UserId))
                .ForMember(d => d.CreationTime, m => m.MapFrom(s => s.CreatedDateTimeUTC));   
        }
    }
}