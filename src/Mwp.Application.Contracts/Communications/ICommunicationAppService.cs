using System;
using Volo.Abp.Application.Services;

namespace Mwp.Communications
{
    public interface ICommunicationAppService : ICrudAppService<CommunicationDto, Guid, GetCommunicationsInput, CommunicationCreateDto, CommunicationUpdateDto>
    {
    }
}