using System;
using Volo.Abp.Application.Services;

namespace Mwp.Engagements
{
    public interface IEngagementAppService : ICrudAppService<EngagementDto, Guid, GetEngagementsInput, EngagementCreateDto, EngagementUpdateDto>
    {
    }
}