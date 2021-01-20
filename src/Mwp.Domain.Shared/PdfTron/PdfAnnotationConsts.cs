namespace Mwp.PdfTron
{
    public static class PdfAnnotationConsts
    {
        private const string DefaultSorting = "{0}CreationTime desc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "PdfAnnotation." : string.Empty);
        }

        public const int FileIdMaxLength = 36;
        public const int AnnotationIdMaxLength = 36;
    }
}