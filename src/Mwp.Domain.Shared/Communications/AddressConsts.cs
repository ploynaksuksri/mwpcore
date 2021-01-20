namespace Mwp.Communications
{
    public static class AddressConsts
    {
        const string DefaultSorting = "{0}Name asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Address." : string.Empty);
        }

        public const int NameMinLength = 1;
        public const int NameMaxLength = 255;
    }
}