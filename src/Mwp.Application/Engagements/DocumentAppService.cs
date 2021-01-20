using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Engagements
{
    [Authorize(MwpPermissions.Documents.Default)]
    public class DocumentAppService : MwpAppService, IDocumentAppService
    {
        readonly IRepository<Document, Guid> _documentRepository;

        public DocumentAppService(IRepository<Document, Guid> documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public virtual async Task<PagedResultDto<DocumentDto>> GetListAsync(GetDocumentsInput input)
        {
            var totalCount = await _documentRepository.GetCountAsync();
            var items = await _documentRepository.GetListAsync();

            return new PagedResultDto<DocumentDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Document>, List<DocumentDto>>(items)
            };
        }

        public virtual async Task<DocumentDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Document, DocumentDto>(await _documentRepository.GetAsync(id));
        }

        [Authorize(MwpPermissions.Documents.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _documentRepository.DeleteAsync(id);
        }

        [Authorize(MwpPermissions.Documents.Create)]
        public virtual async Task<DocumentDto> CreateAsync(DocumentCreateDto input)
        {
            var document = ObjectMapper.Map<DocumentCreateDto, Document>(input);
            document.TenantId = CurrentTenant.Id;
            document = await _documentRepository.InsertAsync(document, autoSave: true);
            return ObjectMapper.Map<Document, DocumentDto>(document);
        }

        [Authorize(MwpPermissions.Documents.Edit)]
        public virtual async Task<DocumentDto> UpdateAsync(Guid id, DocumentUpdateDto input)
        {
            var document = await _documentRepository.GetAsync(id);
            ObjectMapper.Map(input, document);
            document = await _documentRepository.UpdateAsync(document);
            return ObjectMapper.Map<Document, DocumentDto>(document);
        }
    }
}