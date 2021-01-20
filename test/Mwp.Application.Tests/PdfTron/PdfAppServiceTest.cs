using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mwp.Data;
using Mwp.File;
using Mwp.Utilities;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp;
using Xunit;

namespace Mwp.PdfTron
{
    public class PdfAppServiceTest : MwpApplicationTestBase
    {
        private readonly IPdfAppService _pdfAppService;
        private readonly IPdfAnnotationRepository _pdfAnnotationRepository;
        private readonly IFileAppService _fileAppService;
        private readonly Guid _fileId = Guid.Parse("b5a97ee0-dacc-41cd-8870-0b64cacb6193");
        private readonly Guid _annotationId = Guid.Parse("c5a97ee0-dacc-41cd-8870-0b64cacb6193");
        private readonly string _annotation = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><xfdf xmlns=\"http://ns.adobe.com/xfdf/\" xml:space=\"preserve\"><fields /><add /><modify /><delete /></xfdf>";
        public PdfAppServiceTest()
        {
            _pdfAppService = GetRequiredService<IPdfAppService>();
            _pdfAnnotationRepository = GetRequiredService<IPdfAnnotationRepository>();
            _fileAppService = GetRequiredService<IFileAppService>();
        }

        [Fact]
        public async Task GetAnnotations()
        {
            await CreatePdfAnnotations();
            var result = await _pdfAnnotationRepository.GetAnnotationsAsync(_fileId);
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task SaveAnnotation_Success()
        {
            var annotation = new PdfAnnotationDto()
            {
                FileId = _fileId,
                CreatedDateTimeUTC = DateTime.UtcNow,
                UserId = Guid.Parse("4467BE0F-DFD3-22F3-711E-39F5EE79690C"),
                Annotation = _annotation,
                AnnotationId = _annotationId,
                TenantId = null
            };

            annotation.Base64String = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(typeof(PdfAppServiceTest).Assembly, $"Mwp.PdfTron.data.pdf-document.txt");
            var byteArrayData = Convert.FromBase64String(annotation.Base64String);
            var file = await _fileAppService.UploadFile(byteArrayData, "application/pdf", $"PdfAnnotation_{DateTime.UtcNow:yyyyMMdd_HHmmss}", byteArrayData.Length);

            await _pdfAnnotationRepository.InsertAsync(
                new PdfAnnotation()
                {
                    TenantId = annotation.TenantId,
                    CreatorId = annotation.UserId,
                    CreationTime = DateTime.UtcNow,
                    FileId = annotation.FileId,
                    AnnotationId = Guid.Parse(file.FileId),
                    Annotation = annotation.Annotation,
                });

            var result = await _pdfAnnotationRepository.GetAnnotationsAsync(_fileId);
            result.First().PdfAnnotation.FileId.ShouldBe(_fileId);
        }

        [Fact]
        public async Task SaveAnnotation_WhenAnnotationIsNull_ThrowError()
        {
            var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => _pdfAppService.SaveAnnotation(
                new PdfAnnotationDto()
                {
                    FileId = _fileId,
                    AnnotationId = _annotationId,
                    Annotation = null
                }));

            ex.Message.ShouldBe("Annotation cannot be null.");
        }

        [Fact]
        public async Task RestoreVersion()
        {
            await CreatePdfAnnotations();
            PdfAnnotationDto selectedAnnotation = new PdfAnnotationDto()
            {
                FileId = _fileId,
                AnnotationId = _annotationId
            };

            var annotationsBeingDiscarded = await _pdfAnnotationRepository.GetAnnotationsBeingDiscarded(selectedAnnotation.FileId, selectedAnnotation.AnnotationId);
            annotationsBeingDiscarded.Count.ShouldBeGreaterThan(0);

            await _pdfAppService.RestoreVerison(selectedAnnotation);
            var noAnnotationsBeingDiscarded = await _pdfAnnotationRepository.GetAnnotationsBeingDiscarded(selectedAnnotation.FileId, selectedAnnotation.AnnotationId);
            noAnnotationsBeingDiscarded.Count.ShouldBe(0);

            var result = await _pdfAnnotationRepository.GetAnnotationsAsync(_fileId);
            var annotation = result.First();
            annotation.PdfAnnotation.FileId.ShouldBe(_fileId);
            annotation.PdfAnnotation.AnnotationId.ShouldBe(_annotationId);
        }

        private async Task CreatePdfAnnotations()
        {
            await _pdfAnnotationRepository.InsertAsync(new PdfAnnotation
            (
                fileId: _fileId,
                annotationId: _annotationId,
                annotation: _annotation,
                isDiscarded: false
            ));

            await _pdfAnnotationRepository.InsertAsync(new PdfAnnotation
            (
                fileId: _fileId,
                annotationId: Guid.Parse("d5a97ee0-dacc-41cd-8870-0b64cacb6193"),
                annotation: _annotation,
                isDiscarded: false
            ));

            await _pdfAnnotationRepository.InsertAsync(new PdfAnnotation
            (
                fileId: Guid.Parse("b2147f35-bce1-4e33-9ef8-7f85b75a93e4"),
                annotationId: Guid.Parse("c2147f35-bce1-4e33-9ef8-7f85b75a93e4"),
                annotation: _annotation,
                isDiscarded: false
            ));
        }
    }
}