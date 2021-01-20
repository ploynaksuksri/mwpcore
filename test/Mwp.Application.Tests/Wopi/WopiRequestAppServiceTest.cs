using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Mwp.File;
using Mwp.Localization;
using Newtonsoft.Json;
using Shouldly;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using Xunit;

namespace Mwp.Wopi
{
    public class WopiRequestAppServiceTest : MwpApplicationTestBase
    {
        readonly IGuidGenerator GuidGenerator;
        readonly ICurrentUser CurrentUser;

        readonly HttpContext _httpContext;
        readonly IWopiRequestAppService _wopiRequestAppService;
        readonly IFileAppService _fileAppService;
        readonly IWopiFileHistoryRepository _wopiFileHistoryRepo;

        readonly IStringLocalizer<MwpResource> _l;

        readonly string _selfUrl;
        readonly string _clientUrl;

        const string _defaultFileName = "test.docx";
        const string _defaultLockValue = "test-lock-value";

        const string _accessToken =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijd2Vk1zTklTN2RTNnBEaXkya1NLZlEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE1OTEwNjkzMzEsImV4cCI6MTYyMjYwNTMzMSwiaXNzIjoiaHR0cHM6Ly9pZGVudGl0eXNlcnZlci1kZXYuZ28ubXl3b3JrcGFwZXJzLmNvLnVrIiwiYXVkIjoiTXdwIiwiY2xpZW50X2lkIjoiTXdwX0FwcCIsInN1YiI6IjU5OWE5OGQ1LTg0YjktNWIxNy0wYWNmLTM5ZjU2NmE1ZGYxNSIsImF1dGhfdGltZSI6MTU5MTA2OTMzMCwiaWRwIjoibG9jYWwiLCJyb2xlIjoiYWRtaW4iLCJuYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImFkbWluQGFicC5pbyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwic2NvcGUiOlsiTXdwIl0sImFtciI6WyJwd2QiXX0.bFv2W-T5vaMAmZEq-zKsgBHfiQmwL3v_h6moU_bRUWhafd9qxD0Fzs_7FM3dA7sQqRrqrjWChscNXY3JZdFy7hRsW5iU64KSWejK0Vqcohjwf5lYLZWwKIx9I-TXZtO0ve7cqV7yCNpcGtRxm9YHH82I11OZ3UBZC1W6EFjlbmhTSlipa791RyjprpSv10pzNRlyI9iiVxIH3AWVTK-frHkVkPb249Rf6N0skn3d9f7MaQZbA1INnPPwGJ2T7eF4456vkWWAHy4sj0Ea3ca0YO6Tddw5MHCODn5KdtCpw14I4vwObWzziXJW99hcjcISekqB2Z4qD0uyCVuwwNl28Q";

        public WopiRequestAppServiceTest()
        {
            GuidGenerator = GetRequiredService<IGuidGenerator>();
            CurrentUser = GetRequiredService<ICurrentUser>();
            var configuration = GetRequiredService<IConfiguration>();

            var _httpContextAccessor = GetRequiredService<IHttpContextAccessor>();
            _httpContextAccessor.HttpContext = new DefaultHttpContext();
            _httpContext = _httpContextAccessor.HttpContext;
            SetRequestHeader("Authorization", $"Bearer {_accessToken}");

            _fileAppService = GetRequiredService<IFileAppService>();
            _wopiRequestAppService = GetRequiredService<IWopiRequestAppService>();
            _wopiFileHistoryRepo = GetRequiredService<IWopiFileHistoryRepository>();

            _l = GetRequiredService<IStringLocalizer<MwpResource>>();

            _selfUrl = configuration[MwpConsts.SelfUrl].EnsureEndsWith('/');
            _clientUrl = configuration[MwpConsts.ClientUrl].EnsureEndsWith('/');
        }

        #region Common

        [Fact]
        public async Task ProcessWopiRequest_InvalidFileIdFormat_ShouldReturn_BadRequest()
        {
            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest("abc_def_ghi", WopiRequestType.CheckFileInfo);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task ProcessWopiRequest_InvalidWopiProof_ShouldReturn_InternalServerError()
        {
            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest("abc_def", WopiRequestType.CheckFileInfo);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task ProcessWopiRequest_InvalidFileId_ShouldReturn_InternalServerError()
        {
            await Assert.ThrowsAsync<FormatException>(() => _wopiRequestAppService.ProcessWopiRequest("abc_def", WopiRequestType.CheckFileInfo, false));
        }

        [Fact]
        public async Task ProcessWopiRequest_NoFile_ShouldReturn_NotFound()
        {
            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(GuidGenerator.Create().ToString(), WopiRequestType.CheckFileInfo, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ProcessWopiRequest_NoHistoryId_ShouldReturn_NotFound()
        {
            // arrange
            var (fileId, _) = await UploadFile();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest($"{fileId}_{GuidGenerator.Create()}", WopiRequestType.CheckFileInfo, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
        }

        #endregion Common

        #region CheckFileInfo

        [Fact]
        public async Task CheckFileInfo_ShouldReturn_ContentResultOfFileInfo()
        {
            // arrange
            var (fileId, historyId) = await UploadFile();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest($"{fileId}_{historyId}", WopiRequestType.CheckFileInfo, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var content = JsonConvert.DeserializeObject<WopiFileDto>(output.Content);
            content.BaseFileName.ShouldBe(_defaultFileName);
            content.HostViewUrl.ShouldBe($"{_clientUrl}msofficeonline/{fileId}?action=view");
            content.HostEditUrl.ShouldBe($"{_clientUrl}msofficeonline/{fileId}?action=edit");
        }


        [Fact]
        public async Task ProcessWopiRequest_CheckFileInfo_NotSpecifyHistoryId_ShouldReturn_ContentResultOfFileInfo()
        {
            // arrange
            var (fileId, _) = await UploadFile();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.CheckFileInfo, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var content = JsonConvert.DeserializeObject<WopiFileDto>(output.Content);
            content.BaseFileName.ShouldBe(_defaultFileName);
            content.HostViewUrl.ShouldBe($"{_clientUrl}msofficeonline/{fileId}?action=view");
            content.HostEditUrl.ShouldBe($"{_clientUrl}msofficeonline/{fileId}?action=edit");
        }

        #endregion CheckFileInfo

        #region GetFile

        [Fact]
        public async Task GetFile_ShouldReturn_FileContent()
        {
            // arrange
            var (fileId, historyId) = await UploadFile();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest($"{fileId}_{historyId}", WopiRequestType.GetFile, false);

            // assert
            var output = result.ShouldBeOfType<FileContentResult>();
            output.FileContents.Length.ShouldBeGreaterThan(0);
            output.ContentType.ShouldBe(FileUtil.GetMimeMapping(_defaultFileName));
        }

        #endregion GetFile

        #region Lock, GetLock,  RefreshLock, UnlockAndRelock, Unlock

        [Fact]
        public async Task GetLock_Should_AllSuccess()
        {
            var (fileId, _) = await UploadFile();

            // Ensure file is locked
            var currentLockValue = await EnsureFileIsLocked(fileId);
            GetLock(fileId).Result.ShouldBe(currentLockValue);

            // UnlockFile
            await UnlockFile(fileId, currentLockValue);
            GetLock(fileId).Result.ShouldBeNullOrWhiteSpace();

            // LockFile
            var lockValue = "ProcessWopiRequest_GetLock_ShouldReturn_Ok";
            await LockFile(fileId, lockValue);
            GetLock(fileId).Result.ShouldBe(lockValue);

            // RefreshLock
            await RefreshLockFile(fileId, lockValue);

            // UnlockAndRelock
            var newLockValue = "ProcessWopiRequest_GetLock_ShouldReturn_Ok_New";
            await UnlockAndRelockFile(fileId, lockValue, newLockValue);
            GetLock(fileId).Result.ShouldBe(newLockValue);

            // Unlock
            await UnlockFile(fileId, newLockValue);
        }

        #endregion Lock, GetLock,  RefreshLock, UnlockAndRelock, Unlock

        #region PutFile

        [Fact]
        public async Task PutFile_WithSameSession_Should_CreateRevision()
        {
            // arrange
            var (fileId, _) = await UploadFile();
            var currentLock = await EnsureFileIsLocked(fileId);

            SetRequestHeader(WopiRequestHeader.LOCK, currentLock);
            SetRequestHeader(WopiRequestHeader.EDITORS, CurrentUser.Id.ToString());
            SetRequestHeader(WopiRequestHeader.SESSION_ID, "session1");

            AddFileContentToHttpContext();

            // Putfile
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutFile, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
            GetResponseHeader(WopiResponseHeader.ITEM_VERSION).ShouldBe("2.0");

            // Putfile again on same session
            result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutFile, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
            GetResponseHeader(WopiResponseHeader.ITEM_VERSION).ShouldBe("2.1");
        }

        [Fact]
        public async Task PutFile_WithDiffSession_Should_CreateVersion()
        {
            // arrange
            var (fileId, _) = await UploadFile();
            var currentLock = await EnsureFileIsLocked(fileId);

            SetRequestHeader(WopiRequestHeader.LOCK, currentLock);
            SetRequestHeader(WopiRequestHeader.EDITORS, CurrentUser.Id.ToString());
            SetRequestHeader(WopiRequestHeader.SESSION_ID, "session1");

            AddFileContentToHttpContext();

            // Putfile
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutFile, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
            GetResponseHeader(WopiResponseHeader.ITEM_VERSION).ShouldBe("2.0");

            // Putfile again on differrent session
            SetRequestHeader(WopiRequestHeader.SESSION_ID, "session2");

            result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutFile, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
            GetResponseHeader(WopiResponseHeader.ITEM_VERSION).ShouldBe("3.0");
        }

        #endregion PutFile

        #region PutRelativeFile

        [Fact]
        public async Task PutRelativeFile_WithRelativeTarget_Overwrite_FileNameExistAndLocked_ShouldReturn_LockValue()
        {
            // arrange
            var fileName = "Relative_Overwrite_FileNameExistAndLocked.docx";

            var (fileId, _) = await UploadFile(fileName);

            RemoveRequestHeader(WopiRequestHeader.SUGGESTED_TARGET);
            SetRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET, "true");
            SetRequestHeader(WopiRequestHeader.RELATIVE_TARGET, fileName);
            var currentLock = await EnsureFileIsLocked(fileId);

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.Conflict);

            GetResponseHeader(WopiResponseHeader.LOCK).ShouldBe(currentLock);
        }

        [Fact]
        public async Task PutRelativeFile_WithRelativeTarget_NotOverwrite_FileNameExistAndLocked_ShouldReturn_LockValueAndAvailableName()
        {
            // arrange
            var fileName = "Relative_NotOverwrite_FileNameExistAndLocked.docx";
            var (fileId, _) = await UploadFile(fileName);

            RemoveRequestHeader(WopiRequestHeader.SUGGESTED_TARGET);
            SetRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET, "false");
            SetRequestHeader(WopiRequestHeader.RELATIVE_TARGET, fileName);
            var currentLock = await EnsureFileIsLocked(fileId);

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.Conflict);

            var expectedValidFileName = $"{FileUtil.GetFileNameWithoutExtension(fileName)}_1.{FileUtil.GetFileExtension(fileName)}";
            GetResponseHeader(WopiResponseHeader.LOCK).ShouldBe(currentLock);
            GetResponseHeader(WopiResponseHeader.VALID_RELATIVE_TARGET).ShouldBe(expectedValidFileName);
        }

        [Fact]
        public async Task PutRelativeFile_WithRelativeTarget_NotOverwrite_FileNameExistAndUnlocked_ShouldReturn_AvailableName()
        {
            // arrange
            var fileName = "Relative_NotOverwrite_FileNameExistAndUnlocked.docx";

            var (fileId, _) = await UploadFile(fileName);

            RemoveRequestHeader(WopiRequestHeader.SUGGESTED_TARGET);
            SetRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET, "false");
            SetRequestHeader(WopiRequestHeader.RELATIVE_TARGET, fileName);

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.Conflict);

            var expectedValidFileName = $"{FileUtil.GetFileNameWithoutExtension(fileName)}_1.{FileUtil.GetFileExtension(fileName)}";
            GetResponseHeader(WopiResponseHeader.VALID_RELATIVE_TARGET).ShouldBe(expectedValidFileName);
        }

        [Fact]
        public async Task PutRelativeFile_WithRelativeTarget_FileNameNotExist_Should_GetNewFile()
        {
            // arrange
            var (fileId, _) = await UploadFile();

            var newFileName = "Relative_FileNameNotExist.docx";

            RemoveRequestHeader(WopiRequestHeader.SUGGESTED_TARGET);
            SetRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET, "true");
            SetRequestHeader(WopiRequestHeader.RELATIVE_TARGET, newFileName);
            SetRequestQueryString("access_token", _accessToken);

            AddFileContentToHttpContext();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var content = JsonConvert.DeserializeObject<WopiRelativeFileDto>(output.Content);
            var newFileId = content.Id;
            content.Name.ShouldBe(newFileName);
            content.Url.ShouldBe($"{_selfUrl}wopi/files/{newFileId}?access_token={_accessToken}");
            content.HostViewUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=view");
            content.HostEditUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=edit");
        }

        [Fact]
        public async Task PutRelativeFile_WithSuggestedTarget_FileNameNotExsit_GetNewFileWithName()
        {
            // arrange
            var fileName = "Suggested_FileNameNotExsit.ppt";
            var suggestedExt = ".docx";

            var (fileId, _) = await UploadFile(fileName);

            RemoveRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET);
            SetRequestHeader(WopiRequestHeader.SUGGESTED_TARGET, suggestedExt);
            SetRequestQueryString("access_token", _accessToken);

            AddFileContentToHttpContext();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var expectedFileName = $"{FileUtil.GetFileNameWithoutExtension(fileName)}{suggestedExt}";

            var content = JsonConvert.DeserializeObject<WopiRelativeFileDto>(output.Content);
            var newFileId = content.Id;
            content.Name.ShouldBe(expectedFileName);
            content.Url.ShouldBe($"{_selfUrl}wopi/files/{newFileId}?access_token={_accessToken}");
            content.HostViewUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=view");
            content.HostEditUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=edit");
        }

        [Fact]
        public async Task PutRelativeFile_WithSuggestedTarget_FileNameIsExist_Should_GetNewFileWithNextAvailableName()
        {
            // arrange
            var fileName = "Suggested_FileNameIsExist.pptx";

            var (fileId, _) = await UploadFile(fileName);

            RemoveRequestHeader(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET);
            SetRequestHeader(WopiRequestHeader.SUGGESTED_TARGET, fileName);
            SetRequestQueryString("access_token", _accessToken);

            AddFileContentToHttpContext();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.PutRelativeFile, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var expectedFileName = $"{FileUtil.GetFileNameWithoutExtension(fileName)}_1.{FileUtil.GetFileExtension(fileName)}";

            var content = JsonConvert.DeserializeObject<WopiRelativeFileDto>(output.Content);
            var newFileId = content.Id;
            content.Name.ShouldBe(expectedFileName);
            content.Url.ShouldBe($"{_selfUrl}wopi/files/{newFileId}?access_token={_accessToken}");
            content.HostViewUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=view");
            content.HostEditUrl.ShouldBe($"{_clientUrl}msofficeonline/{newFileId}?action=edit");
        }

        #endregion PutRelativeFile

        #region RenameFile

        [Fact]
        public async Task RenameFile_RenameFile_FileNameExist_Should_GetNewHistoryWithNextAvailableName()
        {
            // arrange
            var fileName = "RenameFile_SameFileName.xlsx";

            var (fileId, _) = await UploadFile(fileName);

            SetRequestHeader(WopiRequestHeader.REQUESTED_NAME, FileUtil.GetFileNameWithoutExtension(fileName));

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.RenameFile, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var expectedFileName = $"{FileUtil.GetFileNameWithoutExtension(fileName)}_1";
            var content = JsonConvert.DeserializeObject<WopiRelativeFileDto>(output.Content);
            content.Name.ShouldBe(expectedFileName);

            var history = await GetLatestHistory(fileId);
            history.Version.ShouldBe(2);
            history.Revision.ShouldBe(0);
            history.LastModificationDetail.ShouldBe($"Renamed in Office Online from {fileName} to {expectedFileName}.{FileUtil.GetFileExtension(fileName)}");
        }

        [Fact]
        public async Task RenameFile_RenameFile_FileNameNotExist_Should_GetNewHistoryWithNextAvailableName()
        {
            // arrange
            var fileName = "RenameFile_FileNameNotExist.xls";

            var (fileId, _) = await UploadFile(fileName);

            var newFileName = "RenameFile_FileNameNotExist_V2.xls";
            SetRequestHeader(WopiRequestHeader.REQUESTED_NAME, FileUtil.GetFileNameWithoutExtension(newFileName));

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.RenameFile, false);

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);

            var content = JsonConvert.DeserializeObject<WopiRelativeFileDto>(output.Content);
            content.Name.ShouldBe(FileUtil.GetFileNameWithoutExtension(newFileName));

            var history = await GetLatestHistory(fileId);
            history.Version.ShouldBe(2);
            history.Revision.ShouldBe(0);
            history.LastModificationDetail.ShouldBe(_l["RenamedInOfficeOnline", $"{fileName}", $"{newFileName}"]);
        }

        #endregion RenameFile

        #region DeleteFile

        [Fact]
        public async Task DeleteFile_FileIsLocked_ShouldReturn_LockValue()
        {
            // arrange
            var (fileId, _) = await UploadFile();
            var currentLock = await EnsureFileIsLocked(fileId);

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.DeleteFile, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.Conflict);
            GetResponseHeader(WopiResponseHeader.LOCK).ShouldBe(currentLock);
        }

        [Fact]
        public async Task DeleteFile_FileUnlocked_ShouldReturn_Ok()
        {
            // arrange
            var (fileId, _) = await UploadFile();

            // act
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.DeleteFile, false);

            // assert
            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        #endregion DeleteFile


        #region Private Methods

        void SetRequestHeader(string header, string value)
        {
            _httpContext.Request.Headers[header] = value;
        }

        void RemoveRequestHeader(string header)
        {
            _httpContext.Request.Headers.Remove(header);
        }

        void SetRequestQueryString(string param, string value)
        {
            var current = _httpContext.Request.QueryString;
            if (current.HasValue)
            {
                var queryStrings = HttpUtility.ParseQueryString(current.Value);
                if (string.IsNullOrEmpty(queryStrings.Get(param)))
                {
                    _httpContext.Request.QueryString = current.Add(param, value);
                }
                else
                {
                    queryStrings.Set(param, value);
                    _httpContext.Request.QueryString = new QueryString($"?{queryStrings}");
                }
            }
            else
            {
                _httpContext.Request.QueryString = QueryString.Create(param, value);
            }
        }

        void AddFileContentToHttpContext()
        {
            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD");
            _httpContext.Request.ContentLength = data.Length;
            _httpContext.Request.Body = new MemoryStream(data);
        }

        string GetResponseHeader(string header)
        {
            var value = _httpContext.Response.Headers[header];
            return value.ToString();
        }

        async Task<(Guid, Guid)> UploadFile(string fileName = null)
        {
            fileName ??= _defaultFileName;

            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD");
            var uploadResult = await _fileAppService.UploadFile(data, FileUtil.GetMimeMapping(fileName), fileName, data.Length);
            uploadResult.ShouldNotBeNull();
            uploadResult.FileId.ShouldNotBeNullOrEmpty();

            var wopiFileId = new Guid(uploadResult.FileId);
            var wopiFileHistory = await GetLatestHistory(wopiFileId);
            return (wopiFileId, wopiFileHistory.Id);
        }

        Task<WopiFileHistory> GetLatestHistory(Guid wopiFileId)
        {
            return _wopiFileHistoryRepo.GetLatestHistory(wopiFileId);
        }

        async Task<string> EnsureFileIsLocked(Guid fileId)
        {
            var currentLock = await GetLock(fileId);
            if (!string.IsNullOrWhiteSpace(currentLock))
            {
                return currentLock;
            }

            await LockFile(fileId, _defaultLockValue);
            return _defaultLockValue;
        }

        async Task<string> GetLock(Guid fileId)
        {
            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.GetLock, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);

            return GetResponseHeader(WopiResponseHeader.LOCK);
        }

        async Task LockFile(Guid fileId, string lockValue)
        {
            SetRequestHeader(WopiRequestHeader.LOCK, lockValue);

            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.Lock, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        async Task UnlockFile(Guid fileId, string lockValue)
        {
            SetRequestHeader(WopiRequestHeader.LOCK, lockValue);

            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.Unlock, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        async Task RefreshLockFile(Guid fileId, string lockValue)
        {
            SetRequestHeader(WopiRequestHeader.LOCK, lockValue);

            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.RefreshLock, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        async Task UnlockAndRelockFile(Guid fileId, string oldLockValue, string newLockValue)
        {
            SetRequestHeader(WopiRequestHeader.OLD_LOCK, oldLockValue);
            SetRequestHeader(WopiRequestHeader.LOCK, newLockValue);

            var result = await _wopiRequestAppService.ProcessWopiRequest(fileId.ToString(), WopiRequestType.UnlockAndRelock, false);

            result.ShouldBeOfType<StatusCodeResult>().StatusCode.ShouldBe((int)HttpStatusCode.OK);
        }

        #endregion Private Methods
    }
}