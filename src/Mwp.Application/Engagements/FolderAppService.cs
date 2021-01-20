using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Engagements
{
    [Authorize(MwpPermissions.Folders.Default)]
    public class FolderAppService : MwpAppService, IFolderAppService
    {
        readonly IRepository<Folder, Guid> _folderRepository;

        public FolderAppService(IRepository<Folder, Guid> folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public virtual async Task<PagedResultDto<FolderDto>> GetListAsync(GetFoldersInput input)
        {
            var totalCount = await _folderRepository.GetCountAsync();
            var items = await _folderRepository.GetListAsync();

            return new PagedResultDto<FolderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Folder>, List<FolderDto>>(items)
            };
        }

        public virtual async Task<FolderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Folder, FolderDto>(await _folderRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Folders.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _folderRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Folders.Create)]
        public virtual async Task<FolderDto> CreateAsync(FolderCreateDto input)
        {
            var folder = ObjectMapper.Map<FolderCreateDto, Folder>(input);
            folder.TenantId = CurrentTenant.Id;
            folder = await _folderRepository.InsertAsync(folder, autoSave: true);
            return ObjectMapper.Map<Folder, FolderDto>(folder);
        }

        [Authorize(MwpPermissions.Folders.Edit)]
        public virtual async Task<FolderDto> UpdateAsync(Guid id, FolderUpdateDto input)
        {
            var folder = await _folderRepository.GetAsync(id);
            ObjectMapper.Map(input, folder);
            folder = await _folderRepository.UpdateAsync(folder);
            return ObjectMapper.Map<Folder, FolderDto>(folder);
        }
    }
}