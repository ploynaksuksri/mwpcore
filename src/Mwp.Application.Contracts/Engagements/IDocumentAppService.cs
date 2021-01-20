using System;
using Volo.Abp.Application.Services;

namespace Mwp.Engagements
{
    public interface IDocumentAppService : ICrudAppService<DocumentDto, Guid, GetDocumentsInput, DocumentCreateDto, DocumentUpdateDto>
    {
    }
}