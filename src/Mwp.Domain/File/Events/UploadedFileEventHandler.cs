using System;
using System.Linq;
using System.Threading.Tasks;
using Mwp.Wopi;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;

namespace Mwp.File.Events
{
    public class UploadedFileEventHandler : ILocalEventHandler<UploadedFileEventData>, ITransientDependency
    {
        private readonly IGuidGenerator GuidGenerator;
        private readonly ICurrentTenant CurrentTenant;
        private readonly ICurrentUser CurrentUser;

        private readonly IRepository<WopiFile> _wopiFileRepository;
        private readonly IRepository<WopiFileHistory> _wopiFileHistoryRepository;

        public UploadedFileEventHandler(
            IGuidGenerator guidGenerator,
            ICurrentTenant currentTenant,
            ICurrentUser currentUser,
            IRepository<WopiFile> wopiFileRepository,
            IRepository<WopiFileHistory> wopiFileHistoryRepository
        )
        {
            GuidGenerator = guidGenerator;
            CurrentTenant = currentTenant;
            CurrentUser = currentUser;

            _wopiFileRepository = wopiFileRepository;
            _wopiFileHistoryRepository = wopiFileHistoryRepository;
        }

        public async Task HandleEventAsync(UploadedFileEventData eventData)
        {
            var uploadResult = eventData.UploadResult;

            var fileExt = FileUtil.GetFileExtension(uploadResult.FileName);

            string[] wopiSupportedFileExts = { "doc", "docx", "xls", "xlsx", "ppt", "pptx", "wopitest", "wopitestx" };

            if (wopiSupportedFileExts.Contains(fileExt))
            {
                await CreateWopiFileAndHistoryRecord(uploadResult.FileId, uploadResult.FileName, uploadResult.FileSize);
            }
        }

        private async Task CreateWopiFileAndHistoryRecord(string uploadedFileId, string fileName, int fileSize)
        {
            // Let wopi file have the same ID as the initial uploaded file so that we don't have to overwrite form submission data
            var wopiFileId = new Guid(uploadedFileId);

            var wopiFile = new WopiFile(wopiFileId)
            {
                TenantId = CurrentTenant.Id,
                CreatorId = CurrentUser.Id
            };
            await _wopiFileRepository.InsertAsync(wopiFile);

            var wopiFileHistory = new WopiFileHistory
            {
                WopiFileId = wopiFileId,
                Version = 1,
                Size = fileSize,
                FileIdInStorage = new Guid(uploadedFileId),
                BaseFileName = fileName,
                LastModificationTime = DateTime.Now,
                LastModificationDetail = $"Uploaded file {fileName}",
                LastModifiedUsers = CurrentUser.Id.ToString()
            };
            await _wopiFileHistoryRepository.InsertAsync(wopiFileHistory);
        }
    }
}