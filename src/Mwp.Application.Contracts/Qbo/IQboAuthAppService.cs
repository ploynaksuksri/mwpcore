using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mwp.Qbo.Dtos;
using Mwp.Xero.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Qbo
{
    public interface IQboAuthAppService : IApplicationService
    {
        Task<AuthoriseUrlDto> GetAuthoriseUrl();

        Task<IActionResult> GetAccessToken(string code, string state, string realmId);

        Task<QboTokenDto> RefreshAccessToken(string companyId);

        Task<QboTokenDto> GetCurrentToken(string companyId);

        Task RemoveConnection(string companyId);
    }
}