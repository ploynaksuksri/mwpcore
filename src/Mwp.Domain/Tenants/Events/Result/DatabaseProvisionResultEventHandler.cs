using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Mwp.CloudService;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Uow;
using Volo.Saas.Tenants;

namespace Mwp.Tenants.Events.Result
{
    public class DatabaseProvisionResultEventHandler : ResourceProvisionResultEventHandler, IDistributedEventHandler<DatabaseProvisionResultEventData>, ITransientDependency
    {
        protected readonly ITenantRepository TenantRepository;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        protected readonly string _defaultUserId;
        protected readonly string _defaultPassword;

        public DatabaseProvisionResultEventHandler(
            IRepository<TenantResource> tenantResourcesRepository,
            ITenantRepository tenantRepository,
            ITenantResourceManager tenantResourceManager,
            IUnitOfWorkManager manager,
            IConfiguration configuration,
            IStringEncryptionService encryptionService)
            : base(tenantResourcesRepository, encryptionService, tenantResourceManager)
        {
            TenantRepository = tenantRepository;
            UnitOfWorkManager = manager;

            _defaultUserId = configuration[TenantResourceConsts.DefaultUserIdSetting];
            _defaultPassword = configuration[TenantResourceConsts.DefaultPasswordSetting];
        }

        public async Task HandleEventAsync(DatabaseProvisionResultEventData provisionResult)
        {
            await ProcessResultAsync(provisionResult);
        }

        protected override async Task ProcessSuccessProvisioning(TenantResource resource, ResouceProvisionResultEventData provisionResult)
        {
            resource.ConnectionString = GetDatabaseConnectionString(
                TenantResourceConsts.DbConnectionStringTemplate,
                provisionResult.Name,
                provisionResult.ServerName);

            using (var unitOfWork = UnitOfWorkManager.Begin())
            {
                var tenant = await TenantResourceManager.UpdateTenantConnectionString(resource.TenantId, resource.ConnectionString);

                await TenantResourceManager.InitialiseTenantDatabase(tenant, provisionResult.AdminEmailAddress, provisionResult.AdminPassword);
                await TenantResourceManager.SetTenantIsActive(tenant.Id, true);

                await unitOfWork.CompleteAsync();
            }
        }

        string GetDatabaseConnectionString(string connectionstring, string databaseName, string serverName)
        {
            var result = connectionstring
                .Replace("{server}", serverName)
                .Replace("{name}", databaseName)
                .Replace("{user_id}", _defaultUserId)
                .Replace("{your_password}", _defaultPassword);

            return EncryptionService.Encrypt(result);
        }
    }
}