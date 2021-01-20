using System;
using System.Threading.Tasks;

namespace Mwp.File
{
    public interface IFileCleanupStorageClient : IFileStorageClient
    {
        Task DeleteTableStorageInAccount(Guid? tenantId = null);

        Task DeleteFileContainersInAccount(Guid? tenantId = null);
    }
}