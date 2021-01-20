using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Mwp.File;
using Mwp.Localization;
using Mwp.Permissions;
using Mwp.Users;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Localization;

namespace Mwp.Wopi
{
    [Authorize(MwpPermissions.Wopi.Default)]
    public class WopiAppService : MwpAppService, IWopiAppService
    {
        readonly HttpContext _httpContext;

        readonly IRepository<AppUser> _userRepository;
        readonly IRepository<WopiFile> _wopiFileRepository;
        readonly IWopiFileHistoryRepository _wopiFileHistoryRepository;

        protected readonly IStringLocalizer<MwpResource> _l;

        readonly WopiDiscovery _wopiDiscovery;
        readonly WopiUrlService _wopiUrlService;
        readonly IFileStorageClient _fileStorageClient;

        public WopiAppService(IHttpContextAccessor httpContextAccessor, IRepository<AppUser> userRepository, IRepository<WopiFile> wopiFileRepository, IWopiFileHistoryRepository wopiFileHistoryRepository, IStringLocalizer<MwpResource> l,
            WopiDiscovery wopiHelper, WopiUrlService wopiUrlService, IFileStorageClient fileStorageClient)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _userRepository = userRepository;
            _wopiFileRepository = wopiFileRepository;
            _wopiFileHistoryRepository = wopiFileHistoryRepository;
            _l = l;
            _wopiDiscovery = wopiHelper;
            _wopiUrlService = wopiUrlService;
            _fileStorageClient = fileStorageClient;
        }

        public async Task<WopiActionDto> GetWopiActionUrl(Guid fileId, Guid? historyId, string actionName)
        {
            if (string.IsNullOrWhiteSpace(actionName))
            {
                throw new ArgumentException();
            }

            var fileHistory = await GetFileHistory(fileId, historyId);

            var fileActions = await _wopiDiscovery.GetFileActions(fileHistory.BaseFileName);
            var action = fileActions.FirstOrDefault(i => i.Name == actionName);
            if (action == null)
            {
                throw new InvalidOperationException();
            }

            var fileIdWithHistoryId = (historyId == null) ? fileId.ToString() : $"{fileId}_{historyId}";
            var userLanguage = await GetUserLanguage();

            var urlsrc = _wopiDiscovery.BuildActionUrl(action, _wopiUrlService.WopiFileOperationApiUrl, fileIdWithHistoryId, userLanguage);

            var accessToken = GetCurrentUserAccessToken();
            var tokenValidTo = new JwtSecurityTokenHandler().ReadToken(accessToken).ValidTo;

            var result = new WopiActionDto
            {
                FileId = fileId,
                FileIdInStorage = fileHistory.FileIdInStorage,
                BaseFileName = fileHistory.BaseFileName,
                UrlSrc = urlsrc,
                AccessToken = accessToken,
                AccessTokenTtl = Convert.ToInt64(tokenValidTo.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds)
            };
            return result;
        }

        public async Task<GetWopiFileHistoryDto> GetWopiFileHistories(Guid fileId)
        {
            var fileHistories = await (from history in _wopiFileHistoryRepository where history.WopiFileId == fileId orderby history.LastModificationTime descending select history).ToDynamicListAsync<WopiFileHistory>();

            var fileHistoryDtos = ObjectMapper.Map<List<WopiFileHistory>, List<WopiFileHistoryDto>>(fileHistories);

            // Find user names
            var allUserIds = GetAllLastModifiedUserIds(fileHistories);
            var userNameMap = GetUserNameMapping(allUserIds);
            FillUserNames(fileHistoryDtos, userNameMap);

            return new GetWopiFileHistoryDto(fileHistoryDtos);
        }

        public async Task<FileCheckoutInfo> GetFileCheckoutInfo(Guid fileId)
        {
            var file = await GetFile(fileId);
            if (file?.CheckoutBy == null)
            {
                return new FileCheckoutInfo();
            }

            var user = await _userRepository.FindAsync(x => x.Id == file.CheckoutBy);
            return new FileCheckoutInfo
            {
                CheckoutByUserId = user?.Id,
                CheckoutTimestamp = file.CheckoutTimestamp,
                CheckoutUsername = string.IsNullOrEmpty(user?.Name) ? "-" : user.Name
            };
        }

        public async Task<Guid> RestoreToHistory(Guid fileId, Guid historyId)
        {
            var file = await GetFile(fileId);
            var latestHistory = await GetFileHistory(fileId);
            var restoreToHistory = await GetFileHistory(fileId, historyId);

            var lastModificationDetail = (restoreToHistory.LastModificationTime == null) ? null : _l["RestoredToVersion", $"{restoreToHistory.LastModificationTime.Value:MMM dd, yyyy h:mm tt}"];

            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = file.Id,
                Version = latestHistory.Version + 1,
                Size = restoreToHistory.Size,
                FileIdInStorage = restoreToHistory.FileIdInStorage,
                BaseFileName = restoreToHistory.BaseFileName,
                LastModificationTime = DateTime.Now,
                LastModificationDetail = lastModificationDetail,
                LastModifiedUsers = CurrentUser.Id.ToString()
            };

            await _wopiFileHistoryRepository.InsertAsync(newFileHistory);

            return newFileHistory.Id;
        }

        public async Task ReplaceFile(Guid fileId, byte[] fileContent, string newFilename = null)
        {
            var file = await GetFile(fileId);
            if ((file.CheckoutBy != null) && (file.CheckoutBy != CurrentUser.Id))
            {
                throw new BusinessException(HttpStatusCode.BadRequest.ToString(), _l["FileCheckedoutByOther"]);
            }

            var fileHistory = await GetFileHistory(fileId, null);

            var uploadResult = await _fileStorageClient.ReplaceFile(fileId, fileContent, newFilename);
            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = file.Id,
                Version = fileHistory.Version + 1,
                FileIdInStorage = new Guid(uploadResult.FileId),
                BaseFileName = fileHistory.BaseFileName,
                Size = fileContent.Length,
                LastModificationDetail = "Replaced",
                LastModificationTime = DateTime.Now,
                LastModifiedUsers = CurrentUser.Id.ToString()
            };
            await _wopiFileHistoryRepository.InsertAsync(newFileHistory);
        }

        public async Task CheckoutFile(Guid fileId)
        {
            var file = await GetFile(fileId);
            if (file == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound.ToString(), _l["FileNotFound"]);
            }

            if (file.CheckoutBy != null)
            {
                throw new BusinessException(HttpStatusCode.BadRequest.ToString(), _l["FileCheckedout"]);
            }

            if ((CurrentUser == null) || (CurrentUser.Id.GetValueOrDefault() == Guid.Empty))
            {
                throw new BusinessException(HttpStatusCode.Forbidden.ToString(), _l["PermissionDenied"]);
            }

            file.CheckoutBy = CurrentUser.Id.GetValueOrDefault();
            file.CheckoutTimestamp = DateTime.Now;
            await _wopiFileRepository.UpdateAsync(file);
        }

        public async Task CheckinFile(Guid fileId)
        {
            var file = await GetFile(fileId);
            await CheckinFile(file);
        }

        async Task CheckinFile(WopiFile file)
        {
            if (file == null)
            {
                throw new BusinessException(HttpStatusCode.NotFound.ToString(), _l["FileNotFound"]);
            }

            if (file.CheckoutBy == null)
            {
                throw new BusinessException(HttpStatusCode.BadRequest.ToString(), _l["FileNotCheckedout"]);
            }

            if ((CurrentUser == null) || (CurrentUser.Id.GetValueOrDefault() == Guid.Empty))
            {
                throw new BusinessException(HttpStatusCode.Forbidden.ToString(), _l["PermissionDenied"]);
            }

            if (file.CheckoutBy != CurrentUser.Id)
            {
                throw new BusinessException(HttpStatusCode.Forbidden.ToString(), _l["FileCheckedoutByOther"]);
            }

            file.CheckoutBy = null;
            file.CheckoutTimestamp = null;
            await _wopiFileRepository.UpdateAsync(file);
        }

        public async Task CheckinAndReplaceFile(Guid fileId, byte[] fileContent)
        {
            var file = await GetFile(fileId);
            await CheckinFile(file);

            var fileHistory = await GetFileHistory(fileId, null);
            var uploadResult = await _fileStorageClient.ReplaceFile(fileId, fileContent);
            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = file.Id,
                Version = fileHistory.Version + 1,
                FileIdInStorage = new Guid(uploadResult.FileId),
                BaseFileName = fileHistory.BaseFileName,
                Size = fileContent.Length,
                LastModificationDetail = "Checkin",
                LastModificationTime = DateTime.Now,
                LastModifiedUsers = CurrentUser.Id.ToString()
            };
            await _wopiFileHistoryRepository.InsertAsync(newFileHistory);
        }

        #region Private Methods

        async Task<WopiFile> GetFile(Guid fileId)
        {
            return await _wopiFileRepository.GetAsync(f => f.Id == fileId);
        }

        async Task<WopiFileHistory> GetFileHistory(Guid fileId, Guid? historyId = null)
        {
            if (historyId.HasValue)
            {
                return await _wopiFileHistoryRepository.GetAsync(h => h.Id == historyId);
            }

            var latestHistory = await _wopiFileHistoryRepository.GetLatestHistory(fileId);
            if (latestHistory == null)
            {
                throw new EntityNotFoundException();
            }

            return latestHistory;
        }

        string GetCurrentUserAccessToken()
        {
            var accessToken = _httpContext.Request.Headers["Authorization"];
            accessToken = accessToken.ToString().Substring(7);
            return accessToken;
        }

        List<Guid> GetAllLastModifiedUserIds(List<WopiFileHistory> fileHistories)
        {
            var userIds = new HashSet<Guid>();
            foreach (var history in fileHistories)
            {
                if (!string.IsNullOrWhiteSpace(history.LastModifiedUsers))
                {
                    var userIdStrs = history.LastModifiedUsers.Split(',');
                    foreach (var userIdStr in userIdStrs)
                    {
                        var id = new Guid(userIdStr);
                        if (!userIds.Contains(id))
                        {
                            userIds.Add(id);
                        }
                    }
                }
            }

            return userIds.ToList();
        }

        Dictionary<Guid, string> GetUserNameMapping(List<Guid> userIds)
        {
            var users = (from user in _userRepository where userIds.Contains(user.Id) select new { user.Id, user.Name }).ToList();

            return users.ToDictionary(user => user.Id, user => user.Name);
        }

        void FillUserNames(List<WopiFileHistoryDto> fileHistories, Dictionary<Guid, string> userNameMap)
        {
            foreach (var history in fileHistories)
            {
                if (!string.IsNullOrWhiteSpace(history.LastModifiedUsers))
                {
                    var userNames = "";

                    var userIds = history.LastModifiedUsers.Split(',');
                    for (var i = 0; i < userIds.Length; i++)
                    {
                        var userId = new Guid(userIds[i]);
                        if (userNameMap.ContainsKey(userId))
                        {
                            if (string.IsNullOrWhiteSpace(userNames))
                            {
                                userNames = userNameMap[userId];
                            }
                            else
                            {
                                userNames += ", " + userNameMap[userId];
                            }
                        }
                    }

                    history.LastModifiedUsers = userNames;
                }
            }
        }

        async Task<string> GetUserLanguage()
        {
            var userLanguage = await SettingProvider.GetOrNullAsync(LocalizationSettingNames.DefaultLanguage);

            if ((userLanguage != null) && WopiConsts.LanguageMap.ContainsKey(userLanguage.ToLower()))
            {
                return WopiConsts.LanguageMap[userLanguage.ToLower()];
            }

            return "en-US";
        }

        #endregion
    }
}