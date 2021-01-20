using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Mwp.Wopi;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Tracing;

namespace Mwp.Tracing
{
    [Dependency(ReplaceServices = true)]
    public class MwpAspNetCoreCorrelationIdProvider : ICorrelationIdProvider, ITransientDependency
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected AbpCorrelationIdOptions Options { get; }

        public MwpAspNetCoreCorrelationIdProvider(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AbpCorrelationIdOptions> options)
        {
            HttpContextAccessor = httpContextAccessor;
            Options = options.Value;
        }

        public virtual string Get()
        {
            if (HttpContextAccessor.HttpContext?.Request?.Headers == null)
            {
                return CreateNewCorrelationId();
            }

            string correlationId = HttpContextAccessor.HttpContext.Request.Headers[WopiRequestHeader.CORRELATION_ID];
            correlationId ??= HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName];

            if (correlationId.IsNullOrEmpty())
            {
                lock (HttpContextAccessor.HttpContext.Request.Headers)
                {
                    correlationId = CreateNewCorrelationId();
                    HttpContextAccessor.HttpContext.Request.Headers[Options.HttpHeaderName] = correlationId;
                }
            }

            return correlationId;
        }

        protected virtual string CreateNewCorrelationId()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
