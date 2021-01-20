using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Mwp.Users;

namespace Mwp.PdfTron
{
    public class PdfAnnotationWithNavigationProperties
    {
        public PdfAnnotation PdfAnnotation { get; set; }

        public AppUser AppUser { get; set; }
        
    }
}