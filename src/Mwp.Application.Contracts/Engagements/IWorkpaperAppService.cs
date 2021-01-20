using System;
using Volo.Abp.Application.Services;

namespace Mwp.Engagements
{
    public interface IWorkpaperAppService : ICrudAppService<WorkpaperDto, Guid, GetWorkpapersInput, WorkpaperCreateDto, WorkpaperUpdateDto>
    {
    }
}