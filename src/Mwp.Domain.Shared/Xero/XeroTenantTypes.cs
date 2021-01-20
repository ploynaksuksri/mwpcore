using System.ComponentModel;

namespace Mwp.Xero
{
    public enum XeroTenantTypes
    {
        [Description("ORGANISATION")]
        Orgnisation = 1,

        [Description("COMPANY")]
        Company = 2,

        [Description("PRACTICEMANAGER")]
        PracticeManager = 3
    }
}