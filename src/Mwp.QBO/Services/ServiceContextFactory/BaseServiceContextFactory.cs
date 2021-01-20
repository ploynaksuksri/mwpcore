using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Security;

namespace Mwp.Services.ServiceContextFactory
{
    public abstract class BaseServiceContextFactory
    {
        public virtual ServiceContext Create(string realmId, string accessToken)
        {
            var oauthValidator = new OAuth2RequestValidator(accessToken);
            var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.Message.Response.SerializationFormat = SerializationFormat.Json;
            return serviceContext;
        }
    }
}