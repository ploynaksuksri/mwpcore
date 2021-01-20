using Microsoft.EntityFrameworkCore;
using Mwp.Wopi;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mwp.PdfTron;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace Mwp.EntityFrameworkCore.Repositories.PdfTron
{
    public class PdfAnnotationRepository : EfCoreRepository<MwpDbContext, PdfAnnotation, Guid>, IPdfAnnotationRepository
    {
        public PdfAnnotationRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<PdfAnnotation> GetLatestHistory(Guid fileId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Where((x => x.AnnotationId.Equals(fileId)))
                .OrderByDescending((x => x.CreationTime))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public DateTime GetLatestDateTime(Guid fileId, Guid annotationId, CancellationToken cancellationToken = default)
        {
            return DbSet.Where(e => e.FileId == fileId &&
                e.AnnotationId == annotationId)
                .First().CreationTime;
        }

        public async Task<List<PdfAnnotation>> GetAnnotationsBeingDiscarded(Guid fileId, Guid annotationId)
        {
            var latestDateTime = GetLatestDateTime(fileId, annotationId);

            return await DbSet.Where(e => e.FileId == fileId &&
                e.IsDiscarded == false &&
                DateTime.Compare(e.CreationTime, latestDateTime) > 0)
                .ToListAsync();
        }

        public async Task<List<PdfAnnotationWithNavigationProperties>> GetAnnotationsAsync(Guid fileId, string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0)
        {
            var query = from pdfAnnotation in DbSet
                        join Users in DbContext.Users on pdfAnnotation.CreatorId equals Users.Id into AppUser
                        from appUser in AppUser.DefaultIfEmpty()
                        where pdfAnnotation.IsDiscarded == false && pdfAnnotation.FileId == fileId

                        select new PdfAnnotationWithNavigationProperties
                        {
                            PdfAnnotation = pdfAnnotation,
                            AppUser = appUser
                        };

            query = string.IsNullOrWhiteSpace(sorting)
                        ? query.OrderByDescending(x => x.PdfAnnotation.CreationTime)
                        : from e in query orderby sorting select e;

            return await query.PageBy(skipCount, maxResultCount).ToListAsync();
        }
    }
}