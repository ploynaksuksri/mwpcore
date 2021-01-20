using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface IPublisherAppService : ICrudAppService<PublisherDto, Guid, GetPublishersInput, PublisherCreateDto, PublisherUpdateDto>
    {
    }
}