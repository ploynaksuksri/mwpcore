namespace Mwp.Content
{
    public static class TitleCategoryConsts
    {
        const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "TitleCategory." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 255;
    }
}