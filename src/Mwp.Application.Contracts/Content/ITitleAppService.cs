using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface ITitleAppService : ICrudAppService<TitleDto, Guid, GetTitlesInput, TitleCreateDto, TitleUpdateDto>
    {
    }
}