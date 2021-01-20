using System.Collections.Generic;

namespace Mwp.Wopi
{
    /// <summary>
    ///     Contains all valid URL placeholders for different WOPI actions
    ///     Used to build correct action URL for iframe to WOPI
    /// </summary>
    public class WopiUrlPlaceholder
    {
        public static List<string> Placeholders = new List<string>
        {
            BUSINESS_USER, DC_LLCC, DISABLE_CHAT, PERFSTATS, UI_LLCC,
            HOST_SESSION_ID, SESSION_CONTEXT, WOPI_SOURCE, ACTIVITY_NAVIGATION_ID,
            DISABLE_ASYNC, DISABLE_BROADCAST, EMBDDED, FULLSCREEN,
            RECORDING, THEME_ID, VALIDATOR_TEST_CATEGORY
        };

        public const string UI_LLCC = "<ui=UI_LLCC&>";
        public const string DC_LLCC = "<rs=DC_LLCC&>";
        public const string DISABLE_CHAT = "<dchat=DISABLE_CHAT&>";
        public const string HOST_SESSION_ID = "<hid=HOST_SESSION_ID&>";
        public const string SESSION_CONTEXT = "<sc=SESSION_CONTEXT&>";
        public const string WOPI_SOURCE = "<wopisrc=WOPI_SOURCE&>";
        public const string PERFSTATS = "<showpagestats=PERFSTATS&>";
        public const string BUSINESS_USER = "<IsLicensedUser=BUSINESS_USER&>";
        public const string ACTIVITY_NAVIGATION_ID = "<actnavid=ACTIVITY_NAVIGATION_ID&>";

        public const string DISABLE_ASYNC = "<na=DISABLE_ASYNC&>";
        public const string DISABLE_BROADCAST = "<vp=DISABLE_BROADCAST&>";
        public const string EMBDDED = "<e=EMBEDDED&>";
        public const string FULLSCREEN = "<fs=FULLSCREEN&>";
        public const string RECORDING = "<rec=RECORDING&>";
        public const string THEME_ID = "<thm=THEME_ID&>";
        public const string VALIDATOR_TEST_CATEGORY = "<testcategory=VALIDATOR_TEST_CATEGORY>";
    }
}