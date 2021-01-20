using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.TitleCategories.Default)]
    public class TitleCategoryAppService : MwpAppService, ITitleCategoryAppService
    {
        readonly IRepository<TitleCategory, Guid> _titleCategoryRepository;

        public TitleCategoryAppService(IRepository<TitleCategory, Guid> titleCategoryRepository)
        {
            _titleCategoryRepository = titleCategoryRepository;
        }

        public virtual async Task<PagedResultDto<TitleCategoryDto>> GetListAsync(GetTitleCategoriesInput input)
        {
            var totalCount = await _titleCategoryRepository.GetCountAsync();
            var items = await _titleCategoryRepository.GetListAsync();

            return new PagedResultDto<TitleCategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TitleCategory>, List<TitleCategoryDto>>(items)
            };
        }

        public virtual async Task<TitleCategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<TitleCategory, TitleCategoryDto>(await _titleCategoryRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.TitleCategories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _titleCategoryRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.TitleCategories.Create)]
        public virtual async Task<TitleCategoryDto> CreateAsync(TitleCategoryCreateDto input)
        {
            var titleCategory = ObjectMapper.Map<TitleCategoryCreateDto, TitleCategory>(input);
            titleCategory.TenantId = CurrentTenant.Id;
            titleCategory = await _titleCategoryRepository.InsertAsync(titleCategory, autoSave: true);
            return ObjectMapper.Map<TitleCategory, TitleCategoryDto>(titleCategory);
        }

        [Authorize(MwpPermissions.TitleCategories.Edit)]
        public virtual async Task<TitleCategoryDto> UpdateAsync(Guid id, TitleCategoryUpdateDto input)
        {
            var titleCategory = await _titleCategoryRepository.GetAsync(id);
            ObjectMapper.Map(input, titleCategory);
            titleCategory = await _titleCategoryRepository.UpdateAsync(titleCategory);
            return ObjectMapper.Map<TitleCategory, TitleCategoryDto>(titleCategory);
        }
    }
}