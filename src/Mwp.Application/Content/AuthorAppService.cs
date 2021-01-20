using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Content
{
    [Authorize(MwpPermissions.Authors.Default)]
    public class AuthorAppService : MwpAppService, IAuthorAppService
    {
        readonly IRepository<Author, Guid> _authorRepository;

        public AuthorAppService(IRepository<Author, Guid> authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public virtual async Task<PagedResultDto<AuthorDto>> GetListAsync(GetAuthorsInput input)
        {
            var totalCount = await _authorRepository.GetCountAsync();
            var items = await _authorRepository.GetListAsync();

            return new PagedResultDto<AuthorDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Author>, List<AuthorDto>>(items)
            };
        }

        public virtual async Task<AuthorDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Author, AuthorDto>(await _authorRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Authors.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _authorRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Authors.Create)]
        public virtual async Task<AuthorDto> CreateAsync(AuthorCreateDto input)
        {
            var author = ObjectMapper.Map<AuthorCreateDto, Author>(input);
            author.TenantId = CurrentTenant.Id;
            author = await _authorRepository.InsertAsync(author, autoSave: true);
            return ObjectMapper.Map<Author, AuthorDto>(author);
        }

        [Authorize(MwpPermissions.Authors.Edit)]
        public virtual async Task<AuthorDto> UpdateAsync(Guid id, AuthorUpdateDto input)
        {
            var author = await _authorRepository.GetAsync(id);
            ObjectMapper.Map(input, author);
            author = await _authorRepository.UpdateAsync(author);
            return ObjectMapper.Map<Author, AuthorDto>(author);
        }
    }
}