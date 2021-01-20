using System;

namespace Mwp.Wopi
{
    /// <summary>
    ///     Model represent data suppose to return for mehod CheckFileInfo of WOPI service
    ///     https://wopi.readthedocs.io/projects/wopirest/en/latest/files/CheckFileInfo.html
    /// </summary>
    public class WopiRelativeFileDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string HostEditUrl { get; set; }

        public string HostViewUrl { get; set; }
    }
}