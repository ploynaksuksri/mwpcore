namespace Mwp.Wopi
{
    /// <summary>
    ///     Model represent data suppose to return for mehod CheckFileInfo of WOPI service
    ///     https://wopi.readthedocs.io/projects/wopirest/en/latest/files/CheckFileInfo.html
    /// </summary>
    public class WopiFileDto
    {
        #region Required response properties

        public string BaseFileName { get; set; }
        public string OwnerId { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
        public string Version { get; set; }

        #endregion

        #region WOPI host capabilities properties

        public bool SupportsContainers => false;

        public bool SupportsDeleteFile { get; set; } = false;

        public bool SupportsExtendedLockLength => true;

        public bool SupportsFolders => false;

        public bool SupportsGetLock => true;

        public bool SupportsLocks => true;
        public bool SupportsRename { get; set; } = false;
        public bool SupportsUpdate => true;

        public bool SupportsUserInfo => false;

        #endregion

        #region User metadata properties

        public bool LicenseCheckForEditIsEnabled => true;
        public string UserFriendlyName { get; set; }

        #endregion

        #region User permissions properties

        //public bool ReadOnly { get; set; } = false;
        public bool RestrictedWebViewOnly => false;

        public bool UserCanAttend => true; //Broadcast only
        public bool UserCanNotWriteRelative { get; set; } = true;
        public bool UserCanPresent => true; //Broadcast only
        public bool UserCanRename { get; set; } = false;
        public bool UserCanWrite { get; set; } = false;

        #endregion

        #region File URL properties

        public string CloseUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string FileVersionUrl { get; set; }
        public string HostEditUrl { get; set; }
        public string HostEmbeddedViewUrl { get; set; }
        public string HostViewUrl { get; set; }

        #endregion

        #region PostMessage properties for web-based WOPI clients

        public string BreadcrumbBrandName => MwpConsts.CompanyName;
        public string BreadcrumbBrandUrl { get; set; }

        #endregion

        #region Other miscellaneous properties

        public string LastModifiedTime { get; set; }

        #endregion
    }
}