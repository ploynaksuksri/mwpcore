using System;
using System.Collections.Generic;
using System.Text;
using Mwp.Wopi;

namespace Mwp.PdfTron
{
    public class GetAnnotationsDto
    {
        public List<PdfAnnotationDto> PdfAnnotations { get; set; }

        public GetAnnotationsDto(List<PdfAnnotationDto> pdfAnnotations)
        {
            PdfAnnotations = pdfAnnotations;
        }
    }
}