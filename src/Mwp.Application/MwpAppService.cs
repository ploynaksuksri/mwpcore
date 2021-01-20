using Mwp.Localization;
using Volo.Abp.Application.Services;

namespace Mwp
{
    /* Inherit your application services from this class.
     */
    public abstract class MwpAppService : ApplicationService
    {
        protected MwpAppService()
        {
            LocalizationResource = typeof(MwpResource);
        }
    }
}