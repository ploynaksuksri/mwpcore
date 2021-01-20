using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.Titles.Default)]
    public class TitleAppService : MwpAppService, ITitleAppService
    {
        readonly IRepository<Title, Guid> _titleRepository;

        public TitleAppService(IRepository<Title, Guid> titleRepository)
        {
            _titleRepository = titleRepository;
        }

        public virtual async Task<PagedResultDto<TitleDto>> GetListAsync(GetTitlesInput input)
        {
            var totalCount = await _titleRepository.GetCountAsync();
            var items = await _titleRepository.GetListAsync();

            return new PagedResultDto<TitleDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Title>, List<TitleDto>>(items)
            };
        }

        public virtual async Task<TitleDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Title, TitleDto>(await _titleRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Titles.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _titleRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Titles.Create)]
        public virtual async Task<TitleDto> CreateAsync(TitleCreateDto input)
        {
            var title = ObjectMapper.Map<TitleCreateDto, Title>(input);
            title.TenantId = CurrentTenant.Id;
            title = await _titleRepository.InsertAsync(title, autoSave: true);
            return ObjectMapper.Map<Title, TitleDto>(title);
        }

        [Authorize(MwpPermissions.Titles.Edit)]
        public virtual async Task<TitleDto> UpdateAsync(Guid id, TitleUpdateDto input)
        {
            var title = await _titleRepository.GetAsync(id);
            ObjectMapper.Map(input, title);
            title = await _titleRepository.UpdateAsync(title);
            return ObjectMapper.Map<Title, TitleDto>(title);
        }
    }
}