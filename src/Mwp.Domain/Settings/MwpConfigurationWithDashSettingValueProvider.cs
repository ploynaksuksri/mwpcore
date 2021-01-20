using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Settings;

namespace Mwp.Settings
{
    public class MwpConfigurationWithDashSettingValueProvider : ISettingValueProvider, ITransientDependency
    {
        public const string ConfigurationNamePrefix = "Settings:";

        public const string ProviderName = "Mwp";

        public string Name => ProviderName;

        protected IConfiguration Configuration { get; }

        public MwpConfigurationWithDashSettingValueProvider(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual Task<string> GetOrNullAsync(SettingDefinition setting)
        {
            return Task.FromResult(Configuration[ConfigurationNamePrefix + setting.Name.Replace(".", "-")]);
        }

        public Task<List<SettingValue>> GetAllAsync(SettingDefinition[] settings)
        {
            return Task.FromResult(settings.Select(x =>
                    new SettingValue(x.Name, Configuration[ConfigurationNamePrefix + x.Name.Replace(".", "-")])).ToList()
            );
        }
    }
}