using Mwp.Data;
using Mwp.Utilities;
using Volo.Abp.DependencyInjection;

namespace Mwp.EntityFrameworkCore
{
    public class MwpRollbackHelper : IMwpRollbackHelper, ITransientDependency
    {
        public string GetRollbackScript(string rollbackFileName)
        {
            var rollbackScript = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                typeof(MwpRollbackHelper).Assembly,
                $"Mwp.Rollback.Scripts.{rollbackFileName}.sql");
            return rollbackScript;
        }
    }
}