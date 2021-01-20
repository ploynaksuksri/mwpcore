using Microsoft.EntityFrameworkCore;
using Mwp.Tenants;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Mwp.EntityFrameworkCore
{
    public static class MwpDbContextRegistrationOptionsBuilderExtension
    {
        public static void ConfigureMwpEntityDetails(this IAbpDbContextRegistrationOptionsBuilder options)
        {
            //Tenant
            ConfigureTenantEx(options);
            ConfigureTenantResource(options);
        }

        static void ConfigureTenantEx(IAbpDbContextRegistrationOptionsBuilder options)
        {
            options.Entity<TenantEx>(opt =>
            {
                opt.DefaultWithDetailsFunc = q =>
                {
                    return q.Include(tx => tx.Entities)
                            .Include(tx => tx.TenantResources)
                            .Include(tx => tx.TenantType)
                            .Include(tx => tx.Tenant)
                            .ThenInclude(t => t.ConnectionStrings);
                };
            });
        }

        static void ConfigureTenantResource(IAbpDbContextRegistrationOptionsBuilder options)
        {
            options.Entity<TenantResource>(opt =>
            {
                opt.DefaultWithDetailsFunc = q =>
                {
                    return q.Include(tr => tr.TenantEx)
                            .Include(tr => tr.CloudServiceLocation)
                            .Include(tr => tr.CloudServiceOption)
                            .ThenInclude(co => co.CloudService)
                            .ThenInclude(cs => cs.CloudServiceType);
                };
            });
        }
    }
}