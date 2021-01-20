using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mwp.File;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Mwp.Wopi
{
    public class WopiDiscovery : ISingletonDependency
    {
        private readonly IConfiguration _config;

        private readonly IDistributedCache<List<WopiAction>> _wopiActionsCache;
        private readonly IDistributedCache<WopiProof> _wopiProofCache;

        private readonly string wopiProofCacheKey = "WopiProof";

        public ILogger Logger { get; set; }

        public WopiDiscovery(
            IConfiguration config,
            IDistributedCache<List<WopiAction>> wopiActionsCache,
            IDistributedCache<WopiProof> wopiProofCache)
        {
            Logger = NullLogger.Instance;

            _config = config;
            _wopiActionsCache = wopiActionsCache;
            _wopiProofCache = wopiProofCache;
        }


        /// <summary>
        ///     Gets actions information for the file from Wopi Discovry
        /// </summary>
        public async Task<List<WopiAction>> GetFileActions(string fileName)
        {
            var fileExt = FileUtil.GetFileExtension(fileName);
            var allowedActionsOfFileType = await GetFileActions(GetWopiFileType(fileExt));

            return allowedActionsOfFileType
                .Where(i => i.Ext == fileExt)
                .OrderBy(i => i.IsDefault)
                .ToList();
        }

        /// <summary>
        ///     Gets WopiProof key from Wopi Discovry
        /// </summary>
        public async Task<WopiProof> GetWopiProof()
        {
            var cache = await _wopiProofCache.GetAsync(wopiProofCacheKey);
            if (cache != null)
            {
                return cache;
            }

            await CreateWopiDiscoveryCache();

            cache = await _wopiProofCache.GetAsync(wopiProofCacheKey);
            if (cache != null)
            {
                return cache;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        ///     Forms the correct action url for the file and host
        /// </summary>
        public string BuildActionUrl(WopiAction action, string wopiFileOperationApiUrl, string fileIdWithVersion, string userLanguage)
        {
            var urlsrc = action.Urlsrc;

            foreach (var ph in WopiUrlPlaceholder.Placeholders)
            {
                if (urlsrc.Contains(ph))
                {
                    // Replace the placeholder value accordingly
                    var queryStr = GetQueryStringValue(ph, wopiFileOperationApiUrl, fileIdWithVersion, userLanguage);
                    if (!string.IsNullOrEmpty(queryStr))
                    {
                        urlsrc = urlsrc.Replace(ph, queryStr + "&");
                    }
                    else
                    {
                        urlsrc = urlsrc.Replace(ph, queryStr);
                    }
                }
            }

            return urlsrc;
        }


        private async Task<List<WopiAction>> GetFileActions(WopiFileType fileType)
        {
            var cache = await _wopiActionsCache.GetAsync(fileType.ToString());
            if (cache != null)
            {
                return cache;
            }

            await CreateWopiDiscoveryCache();

            cache = await _wopiActionsCache.GetAsync(fileType.ToString());
            if (cache != null)
            {
                return cache;
            }

            throw new NotSupportedException();
        }


        private async Task CreateWopiDiscoveryCache()
        {
            var client = new HttpClient();
            using (var response = await client.GetAsync(_config["WopiDiscovery"]))
            {
                if (response.IsSuccessStatusCode)
                {
                    var xmlString = await response.Content.ReadAsStringAsync();

                    var discoveryXml = XDocument.Parse(xmlString);

                    await CreateWopiActionsCache(discoveryXml);
                    await CreateWopiproofCache(discoveryXml);
                }
            }
        }

        private async Task CreateWopiActionsCache(XDocument discoveryXml)
        {
            var xapps = discoveryXml.Descendants("app");
            foreach (var xapp in xapps)
            {
                var xactions = xapp.Descendants("action");
                var appActions = new List<WopiAction>();

                foreach (var xaction in xactions)
                {
                    appActions.Add(new WopiAction
                    {
                        App = xapp.Attribute("name")?.Value,
                        FavIconUrl = xapp.Attribute("favIconUrl")?.Value,
                        CheckLicense = Convert.ToBoolean(xapp.Attribute("checkLicense")?.Value),
                        Name = xaction.Attribute("name")?.Value,
                        Ext = xaction.Attribute("ext") != null ? xaction.Attribute("ext")?.Value : string.Empty,
                        Progid = xaction.Attribute("progid") != null ? xaction.Attribute("progid")?.Value : string.Empty,
                        IsDefault = xaction.Attribute("default") != null,
                        Urlsrc = xaction.Attribute("urlsrc")?.Value,
                        Requires = xaction.Attribute("requires") != null ? xaction.Attribute("requires")?.Value : string.Empty
                    });
                }

                await _wopiActionsCache.SetAsync(xapp.Attribute("name")?.Value, appActions);
            }
        }

        private async Task CreateWopiproofCache(XDocument discoveryXml)
        {
            var xWopiProof = discoveryXml.Descendants("proof-key").FirstOrDefault();

            if (xWopiProof != null)
            {
                var wopiProof = new WopiProof
                {
                    Value = xWopiProof.Attribute("value")?.Value,
                    OldValue = xWopiProof.Attribute("oldvalue")?.Value
                };

                await _wopiProofCache.SetAsync(wopiProofCacheKey, wopiProof);
            }
        }


        private WopiFileType GetWopiFileType(string fileExt)
        {
            switch (fileExt.ToLower())
            {
                case "doc":
                case "docx":
                    return WopiFileType.Word;
                case "xls":
                case "csv":
                case "xlsx":
                    return WopiFileType.Excel;
                case "ppt":
                case "pptx":
                    return WopiFileType.PowerPoint;
                case "wopitest":
                case "wopitestx":
                    return WopiFileType.WopiTest;
                default:
                    return WopiFileType.None;
            }
        }


        private string GetQueryStringValue(string placeholder, string wopiFileOperationApiUrl, string fileIdWithVersion, string userLanguage)
        {
            var parameterName = placeholder.Substring(1, placeholder.IndexOf("=", StringComparison.Ordinal));

            switch (placeholder)
            {
                case WopiUrlPlaceholder.BUSINESS_USER:
                    return parameterName + "1";
                case WopiUrlPlaceholder.DC_LLCC:
                case WopiUrlPlaceholder.UI_LLCC:
                    return parameterName + userLanguage;
                case WopiUrlPlaceholder.THEME_ID:
                    return parameterName + "1";
                case WopiUrlPlaceholder.DISABLE_CHAT:
                    return parameterName + "0";
                case WopiUrlPlaceholder.PERFSTATS:
                    return parameterName + "0";
                case WopiUrlPlaceholder.VALIDATOR_TEST_CATEGORY:
                    return parameterName + "OfficeOnline";
                case WopiUrlPlaceholder.WOPI_SOURCE:
                    var wopiSrcUrl = string.Format(wopiFileOperationApiUrl, fileIdWithVersion);
                    return parameterName + wopiSrcUrl;
                case WopiUrlPlaceholder.DISABLE_ASYNC:
                case WopiUrlPlaceholder.DISABLE_BROADCAST:
                    return parameterName + "false";
                case WopiUrlPlaceholder.EMBDDED:
                case WopiUrlPlaceholder.FULLSCREEN:
                case WopiUrlPlaceholder.RECORDING:
                    // These are all broadcast related actions
                    return parameterName + "true";
                default:
                    return "";
            }
        }
    }
}