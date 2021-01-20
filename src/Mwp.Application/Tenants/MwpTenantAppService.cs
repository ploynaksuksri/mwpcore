using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Mwp.CloudService;
using Mwp.Settings;
using Mwp.Tenants.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Settings;
using Volo.Saas.Editions;
using Volo.Saas.Host;
using Volo.Saas.Host.Dtos;
using Volo.Saas.Tenants;

namespace Mwp.Tenants
{
    [RemoteService(false)]
    public class MwpTenantAppService : TenantAppService, IMwpTenantAppService
    {
        private readonly ITenantResourceManager _tenantResourceManager;
        private readonly ICloudServiceOptionRepository _cloudServiceOptionRepository;
        private readonly IRepository<CloudServiceLocation, int> _cloudServiceLocationRepository;
        private readonly ITenantExRepository _tenantExRepository;
        protected readonly IStringEncryptionService EncryptionService;
        private readonly IRepository<TenantResource> _tenantResourceRepository;

        public MwpTenantAppService(
            ITenantRepository tenantRepository,
            IEditionRepository editionRepository,
            ICloudServiceOptionRepository cloudServiceOptionRepository,
            IRepository<CloudServiceLocation, int> cloudServiceLocationRepository,
            IDataSeeder dataSeeder,
            ITenantManager tenantManager,
            ITenantResourceManager tenantResourceManager,
            ITenantExRepository tenantExRepository,
            IStringEncryptionService encryptionService,
            IRepository<TenantResource> tenantResourceRepository)
            : base(tenantRepository, editionRepository, tenantManager, dataSeeder)
        {
            _tenantResourceManager = tenantResourceManager;
            _cloudServiceOptionRepository = cloudServiceOptionRepository;
            _cloudServiceLocationRepository = cloudServiceLocationRepository;
            _tenantExRepository = tenantExRepository;
            EncryptionService = encryptionService;
            _tenantResourceRepository = tenantResourceRepository;
        }

        public new async Task<MwpSaasTenantDto> GetAsync(Guid id)
        {
            using (CurrentTenant.Change(null))
            {
                var originalTenantDto = await base.GetAsync(id);
                var tenant = ObjectMapper.Map<SaasTenantDto, MwpSaasTenantDto>(originalTenantDto);

                FillTenantResourceInfo(tenant);

                return tenant;
            }
        }

        public new async Task<PagedResultDto<MwpSaasTenantDto>> GetListAsync(GetTenantsInput input)
        {
            var parentTenantId = CurrentTenant.Id;

            using (CurrentTenant.Change(null))
            {
                var count = await _tenantExRepository.GetCountAsync(input.Filter, parentTenantId);
                var list = await _tenantExRepository.GetListAsync(
                    input.Sorting,
                    input.MaxResultCount,
                    input.SkipCount,
                    input.Filter,
                    parentTenantId,
                    true
                );

                var tenantDtos = ObjectMapper.Map<List<TenantEx>, List<MwpSaasTenantDto>>(list);

                if (input.GetEditionNames)
                {
                    await FillEditionName(tenantDtos);
                }

                await FillTenantResourceInfo(tenantDtos);

                return new PagedResultDto<MwpSaasTenantDto>(count, tenantDtos);
            }
        }

        public override async Task<SaasTenantDto> CreateAsync(SaasTenantCreateDto input)
        {
            var tenant = await TenantManager.CreateAsync(input.Name, input.EditionId);
            input.MapExtraPropertiesTo(tenant);
            await TenantRepository.InsertAsync(tenant);

            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<Tenant, SaasTenantDto>(tenant);
        }

        [Authorize(SaasHostPermissions.Tenants.Create)]
        public async Task<SaasTenantDto> CreateAsync(MwpSaasTenantCreateDto input)
        {
            var isFreeTrial = await SettingProvider.GetAsync<bool>(MwpSettings.IsFreeTrial);

            await ValidateInput(input, isFreeTrial);

            var parentTenantId = CurrentTenant.Id;

            using (CurrentTenant.Change(null))
            {
                var tenantCreateDto = ObjectMapper.Map<MwpSaasTenantCreateDto, SaasTenantCreateDto>(input);
                var tenantDto = await CreateAsync(tenantCreateDto);

                var tenantEx = new TenantEx(tenantDto.Id, parentTenantId, false);
                await _tenantExRepository.InsertAsync(tenantEx);
                await CurrentUnitOfWork.SaveChangesAsync();

                var provisionRequest = CreateTenantResourceRequest(input, tenantDto.Id, tenantEx.Id, isFreeTrial);
                await _tenantResourceManager.ProvideTenantResources(provisionRequest);

                return tenantDto;
            }
        }

        public override async Task<SaasTenantDto> UpdateAsync(Guid id, SaasTenantUpdateDto input)
        {
            using (CurrentTenant.Change(null))
            {
                return await base.UpdateAsync(id, input);
            }
        }

        public override async Task DeleteAsync(Guid id)
        {
            using (CurrentTenant.Change(null))
            {
                await base.DeleteAsync(id);
            }
        }

        public override async Task<string> GetDefaultConnectionStringAsync(Guid id)
        {
            using (CurrentTenant.Change(null))
            {
                var encryptedConnectionstring = await base.GetDefaultConnectionStringAsync(id);
                return EncryptionService.Decrypt(encryptedConnectionstring);
            }
        }

        public override async Task UpdateDefaultConnectionStringAsync(Guid id, string defaultConnectionString)
        {
            using (CurrentTenant.Change(null))
            {
                var encryptedConnectionString = EncryptionService.Encrypt(defaultConnectionString);
                await base.UpdateDefaultConnectionStringAsync(id, encryptedConnectionString);
            }
        }

        public override async Task DeleteDefaultConnectionStringAsync(Guid id)
        {
            using (CurrentTenant.Change(null))
            {
                await base.DeleteDefaultConnectionStringAsync(id);
            }
        }

        #region private methods

        private TenantResourceRequest CreateTenantResourceRequest(MwpSaasTenantCreateDto input, Guid tenantId, Guid tenantExId, bool isFreeTrial)
        {
            return new TenantResourceRequest
            {
                TenantId = tenantId,
                TenantExId = tenantExId,
                TenantName = input.Name,
                LocationId = input.LocationId,
                IsFreeTrial = isFreeTrial,
                DatabaseOptionId = input.DatabaseOptionId.Value,
                StorageOptionId = input.StorageOptionId.Value,
                AdminEmailAddress = input.AdminEmailAddress,
                AdminPassword = input.AdminPassword
            };
        }

        private void CheckResourceOptionsValue(MwpSaasTenantCreateDto input, bool isFreeTrial)
        {
            if (!input.DatabaseOptionId.HasValue || isFreeTrial)
            {
                input.DatabaseOptionId = (int)CloudServiceOptions.DatabaseBasic;
            }

            if (!input.StorageOptionId.HasValue || isFreeTrial)
            {
                input.StorageOptionId = (int)CloudServiceOptions.StorageStandard;
            }
        }

        private async Task FillEditionName(List<MwpSaasTenantDto> tenantDtos)
        {
            var editions = await EditionRepository.GetListAsync();
            foreach (var tenant in tenantDtos)
            {
                var edition = editions.FirstOrDefault(e => e.Id == tenant.EditionId);
                tenant.EditionName = edition?.DisplayName;
            }
        }

        private void FillTenantResourceInfo(MwpSaasTenantDto tenantDto)
        {
            var tenantResources = _tenantResourceRepository.WithDetails().Where(tr => tr.TenantId == tenantDto.Id).ToList();
            AssignTenantResourceInfo(tenantDto, tenantResources, false);
        }

        private async Task FillTenantResourceInfo(List<MwpSaasTenantDto> tenantDtos)
        {
            var tenantResources = await _tenantResourceRepository.GetListAsync(true);
            foreach (var tenant in tenantDtos)
            {
                var resources = tenantResources.Where(tr => tr.TenantId == tenant.Id).ToList();
                AssignTenantResourceInfo(tenant, resources, true);
            }
        }

        private void AssignTenantResourceInfo(MwpSaasTenantDto tenantDto, List<TenantResource> tenantResources, bool getResourceName)
        {
            var database = tenantResources.FirstOrDefault(tr => tr.CloudServiceOption.CloudService.CloudServiceTypeId == (int)CloudServiceTypes.Databases);
            var storage = tenantResources.FirstOrDefault(tr => tr.CloudServiceOption.CloudService.CloudServiceTypeId == (int)CloudServiceTypes.Storage);

            tenantDto.LocationId = database?.CloudServiceLocationId;
            tenantDto.DatabaseOptionId = database?.CloudServiceOptionId;
            tenantDto.StorageOptionId = storage?.CloudServiceOptionId;

            if (getResourceName)
            {
                tenantDto.LocationName = database?.CloudServiceLocation.LocationName;
                tenantDto.DatabaseOptionName = database?.CloudServiceOption.OptionName;
                tenantDto.StorageOptionName = storage?.CloudServiceOption.OptionName;
            }
        }

        #endregion private methods

        #region input validation

        private async Task ValidateInput(MwpSaasTenantCreateDto input, bool isFreeTrial)
        {
            CheckResourceOptionsValue(input, isFreeTrial);
            await CheckCloudServiceLocationAsync(input.LocationId);
            await CheckCloudServiceOptionAsync(input.DatabaseOptionId.Value);
            await CheckCloudServiceOptionAsync(input.StorageOptionId.Value);
        }

        private async Task CheckCloudServiceOptionAsync(int optionId)
        {
            try
            {
                await _cloudServiceOptionRepository.GetAsync(optionId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw new UserFriendlyException("Selected cloud service option is not available.");
            }
        }

        private async Task CheckCloudServiceLocationAsync(int locationId)
        {
            try
            {
                await _cloudServiceLocationRepository.GetAsync(locationId);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw new UserFriendlyException("Selected cloud service location is not available.");
            }
        }

        #endregion input validation
    }
}