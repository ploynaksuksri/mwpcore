using System;

namespace Mwp.Form
{
    public interface ITenantStorageConnectionProvider
    {
        string GetTenantStorageConnectionString(Guid tenantId);

        void RebuildTenantConnectionStringMap();
    }
}