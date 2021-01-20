using Volo.Abp.Settings;

namespace Mwp.Settings
{
    public class MwpSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MwpSettings.MySetting1));

            context.Add(new SettingDefinition(MwpSettings.IsFreeTrial, "false", isVisibleToClients: true));
        }
    }
}