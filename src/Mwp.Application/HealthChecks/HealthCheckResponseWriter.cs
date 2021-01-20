using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mwp.HealthChecks
{
    public class HealthCheckResponseWriter
    {
        public static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";

            var status = new JProperty("status", result.Status.ToString());
            var resultEntries = new JObject(result.Entries.Select(pair =>
            {
                var (key, value) = pair;
                var _status = new JProperty("status", value.Status.ToString());
                var description = new JProperty("description", value.Description);
                var data = new JProperty("data", new JObject(value.Data.Select(p => new JProperty(p.Key, p.Value))));

                return new JProperty(key, new JObject(_status, description, data));
            }));
            var results = new JProperty("results", resultEntries);

            var json = new JObject(status, results);
            return context.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}