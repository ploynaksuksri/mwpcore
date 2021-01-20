using System;
using Volo.Abp.Application.Services;

namespace Mwp.Engagements
{
    public interface IFolderAppService : ICrudAppService<FolderDto, Guid, GetFoldersInput, FolderCreateDto, FolderUpdateDto>
    {
    }
}