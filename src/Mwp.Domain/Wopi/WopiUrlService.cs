using System;
using Volo.Abp.Domain.Services;
using Volo.Abp.UI.Navigation.Urls;

namespace Mwp.Wopi
{
    public class WopiUrlService : DomainService
    {
        private readonly IAppUrlProvider _webUrlService;

        public WopiUrlService(IAppUrlProvider webUrlService)
        {
            _webUrlService = webUrlService;
        }

        public string WopiFileOperationApiUrl => ApiAddress + "wopi/files/{0}";

        public string WopiFileContentOperationApiUrl => ApiAddress + "wopi/files/{0}/contents";

        public string WopiEmbededViewUrl => UiAddress + "documents/msoffice/{0}";

        public string WopiViewUrl => UiAddress + "msofficeonline/{0}?action=view";

        public string WopiEditUrl => UiAddress + "msofficeonline/{0}?action=edit";

        public string WopiDownloadUrl => UiAddress + "documents/{0}";

        public string WopiFileVersionUrl => UiAddress + "documents/msofficehistory/{0}";

        public string ApiAddress => _webUrlService.GetUrlAsync(MwpConsts.AppName, MwpConsts.SelfUrl).Result.EnsureEndsWith('/');

        public string UiAddress => _webUrlService.GetUrlAsync(MwpConsts.AppName, MwpConsts.ClientUrl).Result.EnsureEndsWith('/');
    }
}