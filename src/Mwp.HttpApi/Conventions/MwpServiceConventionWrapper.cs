using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Volo.Abp.DependencyInjection;
using Volo.Saas.Host;

namespace Mwp.Conventions
{
    [DisableConventionalRegistration]
    public class MwpServiceConventionWrapper : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            // To ignored controller from exposing API endpoints
            var ignoredControllers = new List<Type>
            {
                typeof(TenantController)
            };

            application.Controllers.RemoveAll(c => ignoredControllers.Contains(c.ControllerType));
        }
    }
}
