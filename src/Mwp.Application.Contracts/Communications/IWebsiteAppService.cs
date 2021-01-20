using System;
using Volo.Abp.Application.Services;

namespace Mwp.Communications
{
    public interface IWebsiteAppService : ICrudAppService<WebsiteDto, Guid, GetWebsitesInput, WebsiteCreateDto, WebsiteUpdateDto>
    {
    }
}