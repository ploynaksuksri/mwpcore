using System;

namespace Mwp.PdfTron
{
    public class PdfAnnotationDto
    {
        public Guid? TenantId { get; set; }
        public Guid FileId { get; set; }
        public Guid AnnotationId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedDateTimeUTC { get; set; }
        public string Username { get; set; }
        public string Annotation { get; set; }
        public string Base64String { get; set; }
    }
}