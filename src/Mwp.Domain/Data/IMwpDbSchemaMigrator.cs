using System.Threading.Tasks;

namespace Mwp.Data
{
    public interface IMwpDbSchemaMigrator
    {
        Task MigrateAsync();

        Task RollbackAsync(string script = null);
    }
}