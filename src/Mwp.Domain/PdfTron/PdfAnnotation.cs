using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Mwp.Users;
using Volo.Saas.Tenants;

namespace Mwp.PdfTron
{
    public class PdfAnnotation : AuditedEntity<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public Guid FileId { get; set; }
        public Guid AnnotationId { get; set; }
        public string Annotation { get; set; }
        public bool IsDiscarded { get; set; }

        public PdfAnnotation()
        {

        }

        public PdfAnnotation(Guid fileId, Guid annotationId, string annotation, bool isDiscarded)
        {
            FileId = fileId;
            AnnotationId = annotationId;
            Annotation = annotation;
            IsDiscarded = isDiscarded;
        }
    }
}