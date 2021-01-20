using System;
using Volo.Abp.Application.Services;

namespace Mwp.Content
{
    public interface IAuthorAppService : ICrudAppService<AuthorDto, Guid, GetAuthorsInput, AuthorCreateDto, AuthorUpdateDto>
    {
    }
}