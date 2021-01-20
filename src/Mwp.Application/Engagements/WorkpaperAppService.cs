using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Engagements
{
    [Authorize(MwpPermissions.Workpapers.Default)]
    public class WorkpaperAppService : MwpAppService, IWorkpaperAppService
    {
        readonly IRepository<Workpaper, Guid> _workpaperRepository;

        public WorkpaperAppService(IRepository<Workpaper, Guid> workpaperRepository)
        {
            _workpaperRepository = workpaperRepository;
        }

        public virtual async Task<PagedResultDto<WorkpaperDto>> GetListAsync(GetWorkpapersInput input)
        {
            var totalCount = await _workpaperRepository.GetCountAsync();
            var items = await _workpaperRepository.GetListAsync();

            return new PagedResultDto<WorkpaperDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Workpaper>, List<WorkpaperDto>>(items)
            };
        }

        public virtual async Task<WorkpaperDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Workpaper, WorkpaperDto>(await _workpaperRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Workpapers.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _workpaperRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Workpapers.Create)]
        public virtual async Task<WorkpaperDto> CreateAsync(WorkpaperCreateDto input)
        {
            var workpaper = ObjectMapper.Map<WorkpaperCreateDto, Workpaper>(input);
            workpaper.TenantId = CurrentTenant.Id;
            workpaper = await _workpaperRepository.InsertAsync(workpaper, autoSave: true);
            return ObjectMapper.Map<Workpaper, WorkpaperDto>(workpaper);
        }

        [Authorize(MwpPermissions.Workpapers.Edit)]
        public virtual async Task<WorkpaperDto> UpdateAsync(Guid id, WorkpaperUpdateDto input)
        {
            var workpaper = await _workpaperRepository.GetAsync(id);
            ObjectMapper.Map(input, workpaper);
            workpaper = await _workpaperRepository.UpdateAsync(workpaper);
            return ObjectMapper.Map<Workpaper, WorkpaperDto>(workpaper);
        }
    }
}