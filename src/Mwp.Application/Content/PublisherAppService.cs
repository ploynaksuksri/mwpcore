using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.Publishers.Default)]
    public class PublisherAppService : MwpAppService, IPublisherAppService
    {
        readonly IRepository<Publisher, Guid> _publisherRepository;

        public PublisherAppService(IRepository<Publisher, Guid> publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public virtual async Task<PagedResultDto<PublisherDto>> GetListAsync(GetPublishersInput input)
        {
            var totalCount = await _publisherRepository.GetCountAsync();
            var items = await _publisherRepository.GetListAsync();

            return new PagedResultDto<PublisherDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Publisher>, List<PublisherDto>>(items)
            };
        }

        public virtual async Task<PublisherDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Publisher, PublisherDto>(await _publisherRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Publishers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _publisherRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Publishers.Create)]
        public virtual async Task<PublisherDto> CreateAsync(PublisherCreateDto input)
        {
            var publisher = ObjectMapper.Map<PublisherCreateDto, Publisher>(input);
            publisher.TenantId = CurrentTenant.Id;
            publisher = await _publisherRepository.InsertAsync(publisher, autoSave: true);
            return ObjectMapper.Map<Publisher, PublisherDto>(publisher);
        }

        [Authorize(MwpPermissions.Publishers.Edit)]
        public virtual async Task<PublisherDto> UpdateAsync(Guid id, PublisherUpdateDto input)
        {
            var publisher = await _publisherRepository.GetAsync(id);
            ObjectMapper.Map(input, publisher);
            publisher = await _publisherRepository.UpdateAsync(publisher);
            return ObjectMapper.Map<Publisher, PublisherDto>(publisher);
        }
    }
}