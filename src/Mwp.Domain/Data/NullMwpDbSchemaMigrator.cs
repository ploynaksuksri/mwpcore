using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Mwp.Data
{
    /* This is used if database provider does't define
     * IMwpDbSchemaMigrator implementation.
     */
    public class NullMwpDbSchemaMigrator : IMwpDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }

        public Task RollbackAsync(string script = null)
        {
            return Task.CompletedTask;
        }
    }
}