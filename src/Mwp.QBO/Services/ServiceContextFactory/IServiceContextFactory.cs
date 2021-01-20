using Intuit.Ipp.Core;

namespace Mwp.Services.ServiceContextFactory
{
    public interface IServiceContextFactory
    {
        ServiceContext Create(string realmId, string accessToken);
    }
}