using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace Mwp
{
    [Dependency(ReplaceServices = true)]
    public class MwpBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MyWorkpapers";
    }
}
