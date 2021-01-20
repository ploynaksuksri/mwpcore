namespace Mwp.Wopi
{
    /// <summary>
    ///     Represented the valid WOPI actions from WOPI Discovery
    /// </summary>
    public class WopiAction
    {
        public string App { get; set; }
        public string FavIconUrl { get; set; }
        public bool CheckLicense { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Progid { get; set; }
        public string Requires { get; set; }
        public bool? IsDefault { get; set; }
        public string Urlsrc { get; set; }
    }
}