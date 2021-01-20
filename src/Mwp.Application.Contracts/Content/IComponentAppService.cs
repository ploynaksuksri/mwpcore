using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface IComponentAppService : ICrudAppService<ComponentDto, Guid, GetComponentsInput, ComponentCreateDto, ComponentUpdateDto>
    {
    }
}