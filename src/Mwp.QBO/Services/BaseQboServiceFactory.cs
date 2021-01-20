using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mwp.Qbo;
using Mwp.Services.ServiceContextFactory;

namespace Mwp.Services
{
    public abstract class BaseQboServiceFactory
    {
        protected readonly IServiceContextFactory _serviceContextFactory;

        protected BaseQboServiceFactory(
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            var qboEnvironment = configuration[QboConsts.Environment] ?? QboConsts.EnvironmentProduction;
            var isSandbox = qboEnvironment.Equals(QboConsts.EnvironmentSandbox);

            if (isSandbox)
            {
                _serviceContextFactory = serviceProvider.GetRequiredService<SandboxServiceContextFactory>();
            }
            else
            {
                _serviceContextFactory = serviceProvider.GetRequiredService<ProductionServiceContextFactory>();
            }
        }
    }
}