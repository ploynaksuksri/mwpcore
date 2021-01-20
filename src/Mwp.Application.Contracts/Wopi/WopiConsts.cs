using System.Collections.Generic;

namespace Mwp.Wopi
{
    public static class WopiConsts
    {
        // Map Wopi OVERRIDE to be WopiRequestType
        public static readonly Dictionary<string, WopiRequestType> OverrideRequetTypeMap = new Dictionary<string, WopiRequestType>
        {
            { "GET_LOCK", WopiRequestType.GetLock },
            { "REFRESH_LOCK", WopiRequestType.RefreshLock },
            { "UNLOCK", WopiRequestType.Unlock },
            { "PUT_RELATIVE", WopiRequestType.PutRelativeFile },
            { "RENAME_FILE", WopiRequestType.RenameFile },
            { "DELETE", WopiRequestType.DeleteFile }
        };


        public static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
        {
            { "ar", "ar-SA" },
            { "de", "de-DE" },
            { "es", "es-ES" },
            { "es-mx", "es-mx" },
            { "fr", "fr-FR" },
            { "it", "it-IT" },
            { "pt-br", "pt-BR" },
            { "ru", "ru-Ru" },
            { "tr", "tr-TR" },
            { "vi", "vi-VN" },
            { "zh-hans", "zh-CN" }
        };
    }
}