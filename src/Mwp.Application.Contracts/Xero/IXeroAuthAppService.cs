using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mwp.Xero.Dtos;
using Volo.Abp.Application.Services;

namespace Mwp.Xero
{
    public interface IXeroAuthAppService : IApplicationService
    {
        Task<AuthoriseUrlDto> GetAuthoriseUrl();

        Task<IActionResult> GetAccessToken(string code, string state);

        Task<XeroTokenDto> RefreshAccessToken(Guid xeroTenantId);

        Task<XeroTokenDto> GetCurrentToken(Guid xeroTenantId);

        Task RemoveConnection(string xeroTenantId);
    }
}