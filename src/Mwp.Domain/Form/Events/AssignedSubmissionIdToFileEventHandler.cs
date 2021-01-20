using System;
using System.Threading.Tasks;
using Mwp.File;
using Mwp.Wopi;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;

namespace Mwp.Form.Events
{
    public class AssignedSubmissionIdToFilesEventHandler : ILocalEventHandler<AssignedReferrerIdToFileEventData>,
        ITransientDependency
    {
        private readonly IRepository<WopiFile> _wopiFileRepository;

        public AssignedSubmissionIdToFilesEventHandler(IRepository<WopiFile> wopiFileRepository)
        {
            _wopiFileRepository = wopiFileRepository;
        }

        public async Task HandleEventAsync(AssignedReferrerIdToFileEventData eventData)
        {
            if (eventData.FileIds != null && eventData.FileIds.Length > 0)
            {
                foreach (var fileId in eventData.FileIds)
                {
                    var wopiFile = await _wopiFileRepository.FindAsync(f => f.Id == new Guid(fileId));
                    if (wopiFile != null)
                    {
                        if (eventData.ReferrerType == FileReferrerType.Form)
                        {
                            wopiFile.FormId = eventData.ReferrerId;
                        }
                        else
                        {
                            wopiFile.SubmissionId = eventData.ReferrerId;
                        }

                        await _wopiFileRepository.UpdateAsync(wopiFile);
                    }
                }
            }
        }
    }
}