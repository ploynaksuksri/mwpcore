using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface ITitleCategoryAppService : ICrudAppService<TitleCategoryDto, Guid, GetTitleCategoriesInput, TitleCategoryCreateDto, TitleCategoryUpdateDto>
    {
    }
}