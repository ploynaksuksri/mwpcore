using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Engagements
{
    [Authorize(MwpPermissions.Workbooks.Default)]
    public class WorkbookAppService : MwpAppService, IWorkbookAppService
    {
        readonly IRepository<Workbook, Guid> _workbookRepository;

        public WorkbookAppService(IRepository<Workbook, Guid> workbookRepository)
        {
            _workbookRepository = workbookRepository;
        }

        public virtual async Task<PagedResultDto<WorkbookDto>> GetListAsync(GetWorkbooksInput input)
        {
            var totalCount = await _workbookRepository.GetCountAsync();
            var items = await _workbookRepository.GetListAsync();

            return new PagedResultDto<WorkbookDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Workbook>, List<WorkbookDto>>(items)
            };
        }

        public virtual async Task<WorkbookDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Workbook, WorkbookDto>(await _workbookRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Workbooks.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _workbookRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Workbooks.Create)]
        public virtual async Task<WorkbookDto> CreateAsync(WorkbookCreateDto input)
        {
            var workbook = ObjectMapper.Map<WorkbookCreateDto, Workbook>(input);
            workbook.TenantId = CurrentTenant.Id;
            workbook = await _workbookRepository.InsertAsync(workbook, autoSave: true);
            return ObjectMapper.Map<Workbook, WorkbookDto>(workbook);
        }

        [Authorize(MwpPermissions.Workbooks.Edit)]
        public virtual async Task<WorkbookDto> UpdateAsync(Guid id, WorkbookUpdateDto input)
        {
            var workbook = await _workbookRepository.GetAsync(id);
            ObjectMapper.Map(input, workbook);
            workbook = await _workbookRepository.UpdateAsync(workbook);
            return ObjectMapper.Map<Workbook, WorkbookDto>(workbook);
        }
    }
}