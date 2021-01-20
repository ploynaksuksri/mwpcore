using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;

namespace Mwp.MultiTenancy
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IConnectionStringResolver), typeof(MwpMultiTenantConnectionStringResolver))]
    public class MwpMultiTenantConnectionStringResolver : MultiTenantConnectionStringResolver
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly IServiceProvider _serviceProvider;
        protected readonly IStringEncryptionService EncryptionService;

        public MwpMultiTenantConnectionStringResolver(
            IOptionsSnapshot<AbpDbConnectionOptions> options,
            ICurrentTenant currentTenant,
            IServiceProvider serviceProvider,
            IStringEncryptionService encryptionService)
            : base(options, currentTenant, serviceProvider)
        {
            _currentTenant = currentTenant;
            _serviceProvider = serviceProvider;
            EncryptionService = encryptionService;
        }

        public override string Resolve(string connectionStringName = null)
        {
            //No current tenant, fallback to default logic
            if (_currentTenant.Id == null)
            {
                return GetValue(base.Resolve(connectionStringName));
            }

            using (var serviceScope = _serviceProvider.CreateScope())
            {
                var tenantStore = serviceScope
                    .ServiceProvider
                    .GetRequiredService<ITenantStore>();

                var tenant = tenantStore.Find(_currentTenant.Id.Value);

                if (tenant?.ConnectionStrings == null)
                {
                    return GetValue(base.Resolve(connectionStringName));
                }

                //Requesting default connection string
                if (connectionStringName == null)
                {
                    return GetValue(tenant.ConnectionStrings.Default) ??
                           GetValue(Options.ConnectionStrings.Default);
                }

                //Requesting specific connection string
                var connString = tenant.ConnectionStrings.GetOrDefault(connectionStringName);
                if (connString != null)
                {
                    return GetValue(connString);
                }

                /* Requested a specific connection string, but it's not specified for the tenant.
                 * - If it's specified in options, use it.
                 * - If not, use tenant's default conn string.
                 */

                var connStringInOptions = Options.ConnectionStrings.GetOrDefault(connectionStringName);
                if (connStringInOptions != null)
                {
                    return GetValue(connStringInOptions);
                }

                return GetValue(tenant.ConnectionStrings.Default) ??
                       GetValue(Options.ConnectionStrings.Default);
            }
        }

        private string GetValue(string connectionString)
        {
            try
            {
                return EncryptionService.Decrypt(connectionString);
            }
            catch (FormatException)
            {
                return connectionString; // connectionstring may not be encrypted so, just return the original string.
            }
        }
    }
}