using System;
using System.Collections.Generic;
using System.Linq;
using Mwp.CloudService;
using Mwp.Form;
using Mwp.Tenants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Uow;

namespace Mwp.AzureStorage
{
    public class AzureTenantStorageConnectionProvider : ITenantStorageConnectionProvider, ITransientDependency
    {
        private static readonly Dictionary<Guid, string> _tenantConnectionStringMap = new Dictionary<Guid, string>();
        private readonly IRepository<TenantResource> _tenantResourceRepository;
        private readonly ICurrentTenant _currentTenant;
        private readonly IUnitOfWorkManager _uowManager;
        protected readonly IStringEncryptionService EncryptionService;

        public AzureTenantStorageConnectionProvider(
            ICurrentTenant currentTenant,
            IUnitOfWorkManager uowManager,
            IRepository<TenantResource> tenantResourceRepository,
            IStringEncryptionService encryptionService)
        {
            _currentTenant = currentTenant;
            _tenantResourceRepository = tenantResourceRepository;
            _uowManager = uowManager;
            EncryptionService = encryptionService;
        }

        public string GetTenantStorageConnectionString(Guid tenantId)
        {
            if (_tenantConnectionStringMap.ContainsKey(tenantId))
            {
                return _tenantConnectionStringMap[tenantId];
            }

            RebuildTenantConnectionStringMap();
            if (_tenantConnectionStringMap.ContainsKey(tenantId))
            {
                return _tenantConnectionStringMap[tenantId];
            }

            return null;
        }

        public void RebuildTenantConnectionStringMap()
        {
            if (_tenantConnectionStringMap.Keys.Count > 0)
            {
                return;
            }

            using (_uowManager.Begin())
            {
                List<TenantResource> azStorageTenantResources;
                using (_currentTenant.Change(null))
                {
                    azStorageTenantResources = _tenantResourceRepository
                        .Where(tr => tr.CloudServiceOption.CloudService.CloudServiceTypeId == (int)CloudServiceTypes.Storage)
                        .ToList();
                }

                foreach (var azStorageRes in azStorageTenantResources)
                {
                    var connStr = EncryptionService.Decrypt(azStorageRes.ConnectionString);
                    var tenantId = azStorageRes.TenantId;

                    if (!_tenantConnectionStringMap.ContainsKey(tenantId) && !connStr.IsNullOrWhiteSpace())
                    {
                        _tenantConnectionStringMap.Add(tenantId, connStr);
                    }
                }
            }
        }
    }
}