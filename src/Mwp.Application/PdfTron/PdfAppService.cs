using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Mwp.File;
using Mwp.Localization;
using Mwp.Permissions;
using Volo.Abp;

namespace Mwp.PdfTron
{
    [Authorize(MwpPermissions.Pdf.Default)]
    public class PdfAppService : MwpAppService, IPdfAppService
    {
        private readonly IPdfAnnotationRepository _pdfAnnotationRepository;
        private readonly IFileAppService _fileAppService;
        protected readonly IStringLocalizer<MwpResource> _l;

        public PdfAppService(
            IPdfAnnotationRepository pdfAnnotationRepository,
            IFileAppService fileAppService,
            IStringLocalizer<MwpResource> l)
        {
            _pdfAnnotationRepository = pdfAnnotationRepository;
            _fileAppService = fileAppService;
            _l = l;
        }

        public async Task<GetAnnotationsDto> GetAnnotations(Guid fileId)
        {
            var annotations = await _pdfAnnotationRepository.GetAnnotationsAsync(fileId);
            var result = new List<PdfAnnotationDto>();

            foreach (var annotation in annotations)
            {
                var pdfAnnotationDto = ObjectMapper.Map<PdfAnnotation, PdfAnnotationDto>(annotation.PdfAnnotation);
                pdfAnnotationDto.Username = annotation.AppUser.UserName;
                result.Add(pdfAnnotationDto);

            }
            return new GetAnnotationsDto(result);
        }

        public async Task SaveAnnotation(PdfAnnotationDto annotation)
        {
            if (String.IsNullOrEmpty(annotation.Annotation))
                throw new UserFriendlyException(_l["AnnotationCannotBeNull"]);

            var byteArrayData = Convert.FromBase64String(annotation.Base64String);
            var result = await _fileAppService.UploadFile(byteArrayData, "application/pdf", $"PdfAnnotation_{DateTime.UtcNow:yyyyMMdd_HHmmss}", byteArrayData.Length);

            var pdfAnnotation = ObjectMapper.Map<PdfAnnotationDto, PdfAnnotation>(annotation);
            pdfAnnotation.CreationTime = DateTime.UtcNow;
            pdfAnnotation.AnnotationId = Guid.Parse(result.FileId);
            await _pdfAnnotationRepository.InsertAsync(pdfAnnotation);

        }

        public async Task RestoreVerison(PdfAnnotationDto latestAnnotation)
        {
            var annotationsBeingDiscarded = await _pdfAnnotationRepository.GetAnnotationsBeingDiscarded(latestAnnotation.FileId, latestAnnotation.AnnotationId);
            foreach (var annotation in annotationsBeingDiscarded)
            {
                annotation.IsDiscarded = true;
                await _pdfAnnotationRepository.UpdateAsync(annotation);
            }
        }
    }
}
