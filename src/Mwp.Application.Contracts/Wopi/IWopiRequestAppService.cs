using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;

namespace Mwp.Wopi
{
    public interface IWopiRequestAppService : IApplicationService
    {
        Task<IActionResult> ProcessWopiRequest(string fileIdWithHistoryId, WopiRequestType wopiRequestType, bool validateWopiProof = true);
    }
}