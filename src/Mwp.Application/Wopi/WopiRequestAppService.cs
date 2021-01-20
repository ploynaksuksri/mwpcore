using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Mwp.File;
using Mwp.Form;
using Mwp.Localization;
using Mwp.Permissions;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Wopi
{
    [RemoteService(false)]
    [Authorize(MwpPermissions.Wopi.Default)]
    public partial class WopiRequestAppService : MwpAppService, IWopiRequestAppService
    {
        private readonly HttpContext _httpContext;
        private readonly IRepository<WopiFile> _wopiFileRepository;
        private readonly IWopiFileHistoryRepository _wopiFileHistoryRepository;
        private readonly IFileStorageClient _fileStorageClient;

        private readonly WopiDiscovery _wopiDiscovery;
        private readonly WopiUrlService _wopiUrlService;
        private readonly IHeaderDictionary _requestHeaders;
        private readonly IFormAppService _formAppService;

        protected readonly IStringLocalizer<MwpResource> _l;

        public WopiRequestAppService(
            IHttpContextAccessor httpContextAccessor,
            IRepository<WopiFile> wopiFileRepository,
            IWopiFileHistoryRepository wopiFileHistoryRepository,
            WopiDiscovery wopiHelper,
            WopiUrlService wopiUrlService,
            IFileStorageClient fileStorageClient,
            IFormAppService formAppService,
            IStringLocalizer<MwpResource> l)
        {
            _httpContext = httpContextAccessor.HttpContext;

            _wopiFileRepository = wopiFileRepository;
            _wopiFileHistoryRepository = wopiFileHistoryRepository;
            _fileStorageClient = fileStorageClient;

            _wopiDiscovery = wopiHelper;
            _wopiUrlService = wopiUrlService;

            _requestHeaders = _httpContext.Request.Headers;
            _formAppService = formAppService;

            _l = l;
        }

        public async Task<IActionResult> ProcessWopiRequest(string fileIdWithHistoryId, WopiRequestType wopiRequestType, bool validateWopiProof = true)
        {
            try
            {
                var fileIdParts = fileIdWithHistoryId.Split('_');
                if (fileIdParts.Length > 2)
                {
                    return new StatusCodeResult((int)HttpStatusCode.BadRequest);
                }

                if (validateWopiProof && !await ValidateWopiProof(_httpContext))
                {
                    Logger.LogWarning("WopiProof validation is not passed!!!!");
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }

                var fileId = new Guid(fileIdParts[0]);
                var historyId = fileIdParts.Length == 2 ? new Guid(fileIdParts[1]) : (Guid?)null;

                var file = await GetFile(fileId);

                WopiFileHistory fileHistory;

                switch (wopiRequestType)
                {
                    case WopiRequestType.CheckFileInfo:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await CheckFileInfo(file, fileHistory);

                    case WopiRequestType.GetFile:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await GetFile(fileHistory);

                    case WopiRequestType.Lock:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await Lock(file, fileHistory);

                    case WopiRequestType.GetLock:
                        return await GetLock(file);

                    case WopiRequestType.RefreshLock:
                        return await RefreshLock(file);

                    case WopiRequestType.Unlock:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await Unlock(file, fileHistory);

                    case WopiRequestType.UnlockAndRelock:
                        return await UnlockAndRelock(file);

                    case WopiRequestType.PutFile:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await PutFile(file, fileHistory);

                    case WopiRequestType.PutRelativeFile:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await PutRelativeFile(file, fileHistory);

                    case WopiRequestType.RenameFile:
                        fileHistory = await GetFileHistory(fileId, historyId);
                        return await RenameFile(file, fileHistory);

                    case WopiRequestType.DeleteFile:
                        return await DeleteFile(file);

                    default:
                        return new StatusCodeResult((int)HttpStatusCode.NotImplemented);
                }
            }
            catch (EntityNotFoundException ex)
            {
                Logger.LogError(ex, nameof(ProcessWopiRequest));
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
        }

        #region Private methods

        private async Task<bool> ValidateWopiProof(HttpContext context)
        {
            // Make sure the request has the correct headers
            if (!context.Request.Headers.ContainsKey(WopiRequestHeader.PROOF) ||
                !context.Request.Headers.ContainsKey(WopiRequestHeader.TIME_STAMP))
            {
                return false;
            }

            // TimestampOlderThan20Min
            var timeStamp = long.Parse(context.Request.Headers[WopiRequestHeader.TIME_STAMP].ToString());
            var timeStampDateTime = new DateTime(timeStamp, DateTimeKind.Utc);
            if ((DateTime.UtcNow - timeStampDateTime).TotalMinutes > 20)
            {
                return false;
            }

            // Set the requested proof values
            var requestProof = context.Request.Headers[WopiRequestHeader.PROOF];
            var requestProofOld = string.Empty;
            if (context.Request.Headers.ContainsKey(WopiRequestHeader.PROOF_OLD))
            {
                requestProofOld = context.Request.Headers[WopiRequestHeader.PROOF_OLD];
            }

            // Get the WOPI proof info from Wopi discovery
            var wopiProofPublicKey = await _wopiDiscovery.GetWopiProof();

            // Encode the values into bytes
            var accessTokenBytes = Encoding.UTF8.GetBytes(context.Request.Query["access_token"].ToString());

            var hostUrl = GetAbsolouteUrl(context);
            var hostUrlBytes = Encoding.UTF8.GetBytes(hostUrl.ToUpperInvariant());

            var timeStampBytes = BitConverter.GetBytes(Convert.ToInt64(context.Request.Headers[WopiRequestHeader.TIME_STAMP])).Reverse().ToArray();

            // Build expected proof
            var expected = new List<byte>(
                4 + accessTokenBytes.Length +
                4 + hostUrlBytes.Length +
                4 + timeStampBytes.Length);

            // Add the values to the expected variable
            expected.AddRange(BitConverter.GetBytes(accessTokenBytes.Length).Reverse().ToArray());
            expected.AddRange(accessTokenBytes);
            expected.AddRange(BitConverter.GetBytes(hostUrlBytes.Length).Reverse().ToArray());
            expected.AddRange(hostUrlBytes);
            expected.AddRange(BitConverter.GetBytes(timeStampBytes.Length).Reverse().ToArray());
            expected.AddRange(timeStampBytes);
            var expectedBytes = expected.ToArray();

            return VerifyProofKeys(expectedBytes, requestProof, wopiProofPublicKey.Value) ||
                   VerifyProofKeys(expectedBytes, requestProofOld, wopiProofPublicKey.Value) ||
                   VerifyProofKeys(expectedBytes, requestProof, wopiProofPublicKey.OldValue);
        }

        private string GetAbsolouteUrl(HttpContext context)
        {
            var url = $"{_wopiUrlService.ApiAddress.TrimEnd('/')}{context.Request.Path}{context.Request.QueryString}";
            return url.Replace(":44300", "").Replace(":443", "");
        }

        private bool VerifyProofKeys(byte[] expectedProof, string proofFromRequest, string discoPublicKey)
        {
            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                try
                {
                    rsaProvider.ImportCspBlob(Convert.FromBase64String(discoPublicKey));
                    return rsaProvider.VerifyData(expectedProof, "SHA256", Convert.FromBase64String(proofFromRequest));
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (CryptographicException)
                {
                    return false;
                }
            }
        }

        private Task<WopiFile> GetFile(Guid fileId)
        {
            return _wopiFileRepository.GetAsync(f => f.Id == fileId);
        }

        private async Task<WopiFileHistory> GetFileHistory(Guid fileId, Guid? fileHistoryId)
        {
            if (fileHistoryId.HasValue)
            {
                return await _wopiFileHistoryRepository.GetAsync(h => h.Id == fileHistoryId);
            }

            var latestHistory = await _wopiFileHistoryRepository.GetLatestHistory(fileId);
            if (latestHistory == null)
            {
                throw new EntityNotFoundException();
            }

            return latestHistory;
        }

        #endregion
    }
}