namespace Mwp.Content
{
    public static class ComponentConsts
    {
        const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Component." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 255;
    }
}