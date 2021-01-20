using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface ITemplateAppService : ICrudAppService<TemplateDto, Guid, GetTemplatesInput, TemplateCreateDto, TemplateUpdateDto>
    {
    }
}