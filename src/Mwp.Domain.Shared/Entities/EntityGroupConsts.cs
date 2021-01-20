namespace Mwp.Entities
{
    public static class EntityGroupConsts
    {
        const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "EntityGroup." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 255;
    }
}