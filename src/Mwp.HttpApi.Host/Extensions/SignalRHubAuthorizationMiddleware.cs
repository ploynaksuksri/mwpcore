using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Mwp.Extensions
{
    public class SignalRHubAuthorizationMiddleware
    {
        readonly RequestDelegate _next;
        readonly string _hubPathPrefix;

        public SignalRHubAuthorizationMiddleware(RequestDelegate next, string hubPathPrefix)
        {
            _next = next;
            _hubPathPrefix = hubPathPrefix;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(_hubPathPrefix))
            {
                context.Request.Headers["Authorization"] = JwtBearerDefaults.AuthenticationScheme + " " + accessToken;
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}