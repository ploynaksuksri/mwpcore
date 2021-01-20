using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Mwp.Wopi
{
    public interface IWopiAppService : IApplicationService
    {
        Task<WopiActionDto> GetWopiActionUrl(Guid fileId, Guid? historyId, string actionName);

        Task<GetWopiFileHistoryDto> GetWopiFileHistories(Guid fileId);

        Task<Guid> RestoreToHistory(Guid fileId, Guid historyId);

        Task<FileCheckoutInfo> GetFileCheckoutInfo(Guid fileId);

        Task ReplaceFile(Guid fileId, byte[] fileContent, string newFilename = null);

        Task CheckoutFile(Guid fileId);

        Task CheckinFile(Guid fileId);

        Task CheckinAndReplaceFile(Guid fileId, byte[] fileContent);
    }
}