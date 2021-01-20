using Microsoft.AspNetCore.Builder;

namespace Mwp.Extensions
{
    public static class SignalRHubAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseSignalRHubAuthorization(this IApplicationBuilder builder, string hubPathPrefix)
        {
            return builder.UseMiddleware<SignalRHubAuthorizationMiddleware>(hubPathPrefix);
        }
    }
}