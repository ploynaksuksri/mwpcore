using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;

namespace Mwp.PdfTron
{
    public interface IPdfAppService : IApplicationService
    {
        Task<GetAnnotationsDto> GetAnnotations(Guid fileId);
        Task SaveAnnotation(PdfAnnotationDto annotation);
        Task RestoreVerison(PdfAnnotationDto latestAnnotation);        
    }
}
