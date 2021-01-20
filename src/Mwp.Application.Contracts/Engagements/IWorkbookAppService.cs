using System;
using Volo.Abp.Application.Services;

namespace Mwp.Engagements
{
    public interface IWorkbookAppService : ICrudAppService<WorkbookDto, Guid, GetWorkbooksInput, WorkbookCreateDto, WorkbookUpdateDto>
    {
    }
}