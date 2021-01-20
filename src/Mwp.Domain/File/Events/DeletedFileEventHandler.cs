using System;
using System.Threading.Tasks;
using Mwp.Wopi;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace Mwp.File.Events
{
    public class DeletedFileEventHandler : ILocalEventHandler<DeletedFileEventData>, ITransientDependency
    {
        private readonly IRepository<WopiFile> _wopiFileRepository;
        private readonly IFileStorageClient _fileStorageClient;

        public DeletedFileEventHandler(
            IRepository<WopiFile> wopiFileRepository,
            IFileStorageClient fileStorageClient
        )
        {
            _wopiFileRepository = wopiFileRepository;
            _fileStorageClient = fileStorageClient;
        }

        public async Task HandleEventAsync(DeletedFileEventData eventData)
        {
            var deletedFileId = new Guid(eventData.FileId);

            await MarkWopiFileAsUnused(deletedFileId);
            await MarkWopiHistoryFilesAsUnused(deletedFileId);
        }

        private async Task MarkWopiFileAsUnused(Guid deletedFileId)
        {
            var wopiFile = await _wopiFileRepository.FindAsync(f => f.Id == deletedFileId);
            if (wopiFile != null)
            {
                wopiFile.SubmissionId = null;
                await _wopiFileRepository.UpdateAsync(wopiFile);
            }
        }

        private async Task MarkWopiHistoryFilesAsUnused(Guid deletedFileId)
        {
            var historyFiles = await _fileStorageClient.GetFileIdsByReferredBy(deletedFileId.ToString());
            if (historyFiles != null && historyFiles.Length > 0)
            {
                await _fileStorageClient.UpdateFilesReferredBy(historyFiles, null, FileReferrerType.None);
            }
        }
    }
}