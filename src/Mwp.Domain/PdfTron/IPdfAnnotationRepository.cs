using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Mwp.PdfTron
{
    public interface IPdfAnnotationRepository : IRepository<PdfAnnotation, Guid>
    {
        Task<PdfAnnotation> GetLatestHistory(Guid FileId, CancellationToken cancellationToken = default);
        DateTime GetLatestDateTime(Guid fileId, Guid annotationId, CancellationToken cancellationToken = default);
        Task<List<PdfAnnotation>> GetAnnotationsBeingDiscarded(Guid fileId, Guid annotationId);
        Task<List<PdfAnnotationWithNavigationProperties>> GetAnnotationsAsync(Guid fileId, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0);
    }
}
