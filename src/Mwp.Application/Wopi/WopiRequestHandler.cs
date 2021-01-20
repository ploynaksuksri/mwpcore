using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mwp.File;
using Mwp.Permissions;
using Newtonsoft.Json;

namespace Mwp.Wopi
{
    /// <summary>
    ///     These method are implementation of Wopi protocol methods
    ///     For full documentation, see https://wopi.readthedocs.org/projects/wopirest/en/latest/files/CheckFileInfo.html and
    ///     others file operations documents
    /// </summary>
    public partial class WopiRequestAppService
    {
        private async Task<IActionResult> CheckFileInfo(WopiFile file, WopiFileHistory fileHistory)
        {
            var fileDto = ObjectMapper.Map<WopiFile, WopiFileDto>(file);

            fileDto.BaseFileName = fileHistory.BaseFileName;
            fileDto.Version = $"{fileHistory.Version}.{fileHistory.Revision}";
            fileDto.Size = fileHistory.Size;

            fileDto.DownloadUrl = string.Format(_wopiUrlService.WopiDownloadUrl, fileHistory.FileIdInStorage);
            fileDto.CloseUrl = string.Format(_wopiUrlService.WopiEmbededViewUrl, file.Id);

            fileDto.FileVersionUrl = string.Format(_wopiUrlService.WopiFileVersionUrl, file.Id);
            fileDto.HostEmbeddedViewUrl = string.Format(_wopiUrlService.WopiEmbededViewUrl, file.Id);

            var fileActions = await _wopiDiscovery.GetFileActions(fileHistory.BaseFileName);
            fileDto.HostViewUrl = fileActions.Any(i => i.Name == "view") ? string.Format(_wopiUrlService.WopiViewUrl, file.Id) : "";
            fileDto.HostEditUrl = fileActions.Any(i => i.Name == "edit") ? string.Format(_wopiUrlService.WopiEditUrl, file.Id) : "";

            fileDto.BreadcrumbBrandUrl = _wopiUrlService.UiAddress;

            fileDto.UserId = CurrentUser.Id.ToString();
            fileDto.UserFriendlyName = CurrentUser.UserName;

            var modificationTime = fileHistory.LastModificationTime ?? file.CreationTime;
            fileDto.LastModifiedTime = modificationTime.ToUniversalTime().ToString("O");

            await AttachFilePermissions(fileDto, file);

            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(fileDto)
            };
        }

        private async Task<IActionResult> GetFile(WopiFileHistory fileHistory)
        {
            var fileFromStorage = await _fileStorageClient.DownloadFileFromStorage(fileHistory.FileIdInStorage.ToString());

            SetResponseHeader(WopiResponseHeader.ITEM_VERSION, $"{fileHistory.Version}.{fileHistory.Revision}");
            return new FileContentResult(fileFromStorage.Data, fileFromStorage.ContentType);
        }

        private async Task<IActionResult> GetLock(WopiFile file)
        {
            if (FileIsLocked(file))
            {
                SetResponseHeader(WopiResponseHeader.LOCK, file.LockValue);
            }
            else
            {
                await ClearLockData(file);
                SetResponseHeader(WopiResponseHeader.LOCK, string.Empty);
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        private async Task<IActionResult> Lock(WopiFile file, WopiFileHistory fileHistory)
        {
            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];

            if (!FileIsLocked(file) || string.Equals(file.LockValue, requestLock))
            {
                await ExtendLock(file, requestLock);
                SetResponseHeader(WopiResponseHeader.ITEM_VERSION, $"{fileHistory.Version}.{fileHistory.Revision}");
                return new StatusCodeResult((int)HttpStatusCode.OK);
            }

            return ReturnUnexpectedLockStatus(_l["FileAlreadyLocked", $"{file.LockValue}"], file.LockValue);
        }

        private async Task<IActionResult> RefreshLock(WopiFile file)
        {
            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];

            var lockMismatchStatus = await VerifyLock(file, requestLock);
            if (lockMismatchStatus != null)
            {
                return lockMismatchStatus;
            }

            await ExtendLock(file);

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        private async Task<IActionResult> Unlock(WopiFile file, WopiFileHistory fileHistory)
        {
            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];

            var lockMismatchStatus = await VerifyLock(file, requestLock);
            if (lockMismatchStatus != null)
            {
                return lockMismatchStatus;
            }

            await ClearLockData(file);

            SetResponseHeader(WopiResponseHeader.ITEM_VERSION, $"{fileHistory.Version}.{fileHistory.Revision}");
            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        private async Task<IActionResult> UnlockAndRelock(WopiFile file)
        {
            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];
            string requestOldLock = _requestHeaders[WopiRequestHeader.OLD_LOCK];

            var lockMismatchStatus = await VerifyLock(file, requestOldLock);
            if (lockMismatchStatus != null)
            {
                return lockMismatchStatus;
            }

            await ExtendLock(file, requestLock);

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [Authorize(MwpPermissions.Wopi.Edit)]
        private async Task<IActionResult> PutFile(WopiFile file, WopiFileHistory fileHistory)
        {
            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];

            var lockMismatchStatus = await VerifyLock(file, requestLock);
            var emptyFile = fileHistory.Size == 0 && string.IsNullOrEmpty(file.LockValue);

            if (lockMismatchStatus != null && !emptyFile)
            {
                return lockMismatchStatus;
            }

            var uploadResult = await UploadFileToStorage(fileHistory.BaseFileName, file.Id);

            string sessionId = _requestHeaders[WopiRequestHeader.SESSION_ID];
            string editors = _requestHeaders[WopiRequestHeader.EDITORS];
            if (!string.IsNullOrEmpty(fileHistory.LastModifiedSessionId) && !string.IsNullOrEmpty(sessionId) &&
                string.Equals(fileHistory.LastModifiedSessionId, sessionId, StringComparison.OrdinalIgnoreCase))
            {
                fileHistory.Revision += 1;
                fileHistory.FileIdInStorage = new Guid(uploadResult.FileId);
                fileHistory.LastModificationDetail = _l["UpdatedInOfficeOnline"];
                fileHistory.LastModifiedUsers = editors;

                await _wopiFileHistoryRepository.UpdateAsync(fileHistory);
                SetResponseHeader(WopiResponseHeader.ITEM_VERSION, $"{fileHistory.Version}.{fileHistory.Revision}");
            }
            else
            {
                var newFileHistory = new WopiFileHistory
                {
                    WopiFileId = file.Id,
                    Version = fileHistory.Version + 1,
                    FileIdInStorage = new Guid(uploadResult.FileId),
                    BaseFileName = uploadResult.FileName,
                    Size = uploadResult.FileSize,
                    LastModificationDetail = _l["UpdatedInOfficeOnline"],
                    LastModificationTime = DateTime.Now,
                    LastModifiedUsers = editors,
                    LastModifiedSessionId = sessionId
                };

                await _wopiFileHistoryRepository.InsertAsync(newFileHistory);
                SetResponseHeader(WopiResponseHeader.ITEM_VERSION, $"{newFileHistory.Version}.{newFileHistory.Revision}");
            }

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        [Authorize(MwpPermissions.Wopi.PutRelative)]
        private async Task<IActionResult> PutRelativeFile(WopiFile file, WopiFileHistory fileHistory)
        {
            var (status, newFileName) = GetNewFileNameForPutRelative(file, fileHistory.BaseFileName);
            if (status.StatusCode != (int)HttpStatusCode.OK)
            {
                return status;
            }

            var uploadResult = await UploadFileToStorage(newFileName, file.SubmissionId);

            var newWopiFile = new WopiFile(new Guid(uploadResult.FileId))
            {
                TenantId = CurrentTenant.Id,
                CreatorId = CurrentUser.Id,
                SubmissionId = file.SubmissionId
            };
            await _wopiFileRepository.InsertAsync(newWopiFile);

            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = newWopiFile.Id,
                Version = 1,
                FileIdInStorage = new Guid(uploadResult.FileId),
                BaseFileName = newFileName,
                Size = uploadResult.FileSize,
                LastModificationTime = DateTime.Now,
                LastModificationDetail = _l["ConvertedInOfficeOnline", $"{fileHistory.BaseFileName}", $"{newFileName}"],
                LastModifiedUsers = CurrentUser.Id.ToString(),
                LastModifiedSessionId = _requestHeaders[WopiRequestHeader.SESSION_ID]
            };
            await _wopiFileHistoryRepository.InsertAsync(newFileHistory);

            await UpdateFileInfoInSubmission(file.SubmissionId, file.Id, uploadResult, newWopiFile.Id);

            // Prepare the Json response
            var accessToken = _httpContext.Request.Query["access_token"].ToString();
            var output = new WopiRelativeFileDto
            {
                Id = newWopiFile.Id,
                Name = newFileName,
                Url = string.Format(_wopiUrlService.WopiFileOperationApiUrl + "?access_token={1}", newWopiFile.Id, accessToken),
                HostViewUrl = string.Format(_wopiUrlService.WopiViewUrl, newWopiFile.Id),
                HostEditUrl = string.Format(_wopiUrlService.WopiEditUrl, newWopiFile.Id)
            };
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(output)
            };
        }

        [Authorize(MwpPermissions.Wopi.Rename)]
        private async Task<IActionResult> RenameFile(WopiFile file, WopiFileHistory fileHistory)
        {
            if (!_requestHeaders.ContainsKey(WopiRequestHeader.REQUESTED_NAME))
            {
                return new StatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            string requestLock = _requestHeaders[WopiRequestHeader.LOCK];

            if (FileIsLocked(file) && file.LockValue != requestLock)
            {
                return ReturnUnexpectedLockStatus(_l["FileAlreadyLocked", $"{file.LockValue}"], file.LockValue);
            }

            var fileExt = FileUtil.GetFileExtension(fileHistory.BaseFileName);
            var newFileName = DecodeFileName(_requestHeaders[WopiRequestHeader.REQUESTED_NAME]);
            newFileName = $"{newFileName}.{fileExt}";

            if (FileNameIsExist(newFileName))
            {
                newFileName = GetAvailableFileName(newFileName);
            }

            var newFileInfo = await CreateFileWithNewName(fileHistory.FileIdInStorage, newFileName);

            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = file.Id,
                Version = fileHistory.Version + 1,
                Revision = 0,
                Size = fileHistory.Size,
                FileIdInStorage = new Guid(newFileInfo.FileId),
                BaseFileName = newFileName,
                LastModificationTime = DateTime.Now,
                LastModificationDetail = _l["RenamedInOfficeOnline", $"{fileHistory.BaseFileName}", $"{newFileName}"],
                LastModifiedUsers = CurrentUser.Id.ToString(),
                LastModifiedSessionId = _requestHeaders[WopiRequestHeader.SESSION_ID]
            };
            await _wopiFileHistoryRepository.InsertAsync(newFileHistory);

            await ExtendLock(file);

            await UpdateFileInfoInSubmission(file.SubmissionId, file.Id, newFileInfo);

            var jsonObj = new { Name = FileUtil.GetFileNameWithoutExtension(newFileName) };
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(jsonObj)
            };
        }

        [Authorize(MwpPermissions.Wopi.Delete)]
        private async Task<IActionResult> DeleteFile(WopiFile file)
        {
            if (FileIsLocked(file))
            {
                return ReturnUnexpectedLockStatus(_l["FileAlreadyLocked", $"{file.LockValue}"], file.LockValue);
            }

            await _wopiFileRepository.DeleteAsync(file);

            return new StatusCodeResult((int)HttpStatusCode.OK);
        }

        #region Private Methods

        private async Task AttachFilePermissions(WopiFileDto fileDto, WopiFile file)
        {
            if (await IsGranted(MwpPermissions.Wopi.Edit))
            {
                fileDto.UserCanWrite = true;
            }

            if (await IsGranted(MwpPermissions.Wopi.Delete))
            {
                fileDto.SupportsDeleteFile = true;
            }

            if (await IsGranted(MwpPermissions.Wopi.Rename))
            {
                fileDto.SupportsRename = true;
                fileDto.UserCanRename = true;
            }

            if (await IsGranted(MwpPermissions.Wopi.PutRelative) && file.SubmissionId.HasValue)
            {
                fileDto.UserCanNotWriteRelative = false;
            }

            if (file.CheckoutBy != null && file.CheckoutBy != CurrentUser.Id)
            {
                fileDto.UserCanWrite = false;
                fileDto.SupportsDeleteFile = false;
                fileDto.SupportsRename = false;
                fileDto.UserCanRename = false;
                fileDto.UserCanNotWriteRelative = true;
            }
        }

        private Task<bool> IsGranted(string permissionName)
        {
            return AuthorizationService.IsGrantedAsync(permissionName);
        }

        private string GetAvailableFileName(string fileName)
        {
            var fileExt = FileUtil.GetFileExtension(fileName);
            var fileNameWithoutExt = FileUtil.GetFileNameWithoutExtension(fileName);
            var suffix = 0;
            string newFileName;
            do
            {
                suffix++;
                newFileName = $"{fileNameWithoutExt}_{suffix}";
            } while (FileNameIsExist($"{newFileName}.{fileExt}"));

            return $"{newFileName}.{fileExt}";
        }

        private bool FileNameIsExist(string fileName)
        {
            return _wopiFileHistoryRepository.Any(f => f.BaseFileName == fileName);
        }

        private bool FileWithTheSameNameIsLocked(string fileName)
        {
            var fileIdsHavingTheSameName = _wopiFileHistoryRepository
                .Where(f => f.BaseFileName == fileName)
                .Select(f => f.WopiFileId).ToList();
            return _wopiFileRepository.Any(f =>
                fileIdsHavingTheSameName.Contains(f.Id) &&
                !string.IsNullOrEmpty(f.LockValue) &&
                f.LockExpires != null && f.LockExpires > DateTime.Now
            );
        }

        private bool FileIsLocked(WopiFile file)
        {
            return !string.IsNullOrEmpty(file.LockValue) && file.LockExpires != null && file.LockExpires > DateTime.Now;
        }

        private async Task ClearLockData(WopiFile file)
        {
            if (!string.IsNullOrEmpty(file.LockValue) || file.LockExpires != null)
            {
                file.LockValue = null;
                file.LockExpires = null;
                await _wopiFileRepository.UpdateAsync(file);
            }
        }

        private string DecodeFileName(string fileName)
        {
            var encodedFilename = HttpUtility.UrlEncode(fileName);
            return HttpUtility.UrlDecode(encodedFilename, Encoding.UTF7);
        }

        private void SetResponseHeader(string headerName, string value)
        {
            value = string.IsNullOrEmpty(value) ? " " : value;

            if (_httpContext.Response.Headers.ContainsKey(headerName))
            {
                _httpContext.Response.Headers[headerName] = value;
            }
            else
            {
                _httpContext.Response.Headers.Add(headerName, value);
            }
        }

        private StatusCodeResult ReturnUnexpectedLockStatus(string reason, string existingLock = null)
        {
            SetResponseHeader(WopiResponseHeader.LOCK_FAILURE_REASON, reason);
            SetResponseHeader(WopiResponseHeader.LOCK, existingLock);
            return new StatusCodeResult((int)HttpStatusCode.Conflict);
        }

        private async Task<StatusCodeResult> VerifyLock(WopiFile file, string expectedLockValue)
        {
            if (!FileIsLocked(file))
            {
                await ClearLockData(file);

                return ReturnUnexpectedLockStatus(_l["FileNotLocked"]);
            }

            if (!string.Equals(file.LockValue, expectedLockValue))
            {
                return ReturnUnexpectedLockStatus(_l["LockMismatch"], file.LockValue);
            }

            return null;
        }

        private async Task<UploadFileResult> CreateFileWithNewName(Guid fileIdInStorage, string newFileName)
        {
            var fileFromStorage = await _fileStorageClient.DownloadFileFromStorage(fileIdInStorage.ToString());

            return await _fileStorageClient.UploadFileToStorage(
                fileFromStorage.Data,
                fileFromStorage.ContentType,
                newFileName,
                fileFromStorage.Data.Length);
        }

        private async Task<UploadFileResult> UploadFileToStorage(string fileName, Guid? wopiFileId)
        {
            var size = _httpContext.Request.ContentLength.GetValueOrDefault();
            var data = new byte[size];
            await _httpContext.Request.Body.ReadAsync(data, 0, data.Length);
            return await _fileStorageClient.ReplaceFile(wopiFileId.GetValueOrDefault(), data, fileName, FileUtil.GetMimeMapping(fileName));
        }


        private (StatusCodeResult, string) GetNewFileNameForPutRelative(WopiFile file, string fileName)
        {
            var hasRelativeTarget = _requestHeaders.ContainsKey(WopiRequestHeader.RELATIVE_TARGET);
            var hasSuggestedTarget = _requestHeaders.ContainsKey(WopiRequestHeader.SUGGESTED_TARGET);

            if (hasRelativeTarget == hasSuggestedTarget) // both cannot be true or false and the same time
            {
                return (new StatusCodeResult((int)HttpStatusCode.NotImplemented), null);
            }

            string newFileName;
            if (hasRelativeTarget)
            {
                newFileName = DecodeFileName(_requestHeaders[WopiRequestHeader.RELATIVE_TARGET]);
                var overwrite = IsOverwriteRelativeTarget();

                if (FileWithTheSameNameIsLocked(newFileName) && overwrite)
                {
                    SetResponseHeader(WopiRequestHeader.LOCK, file.LockValue);
                    return (new StatusCodeResult((int)HttpStatusCode.Conflict), null);
                }

                if (FileNameIsExist(newFileName) && !overwrite)
                {
                    if (FileIsLocked(file))
                    {
                        SetResponseHeader(WopiRequestHeader.LOCK, file.LockValue);
                    }

                    SetResponseHeader(WopiRequestHeader.VALID_RELATIVE_TARGET, GetAvailableFileName(newFileName));
                    return (new StatusCodeResult((int)HttpStatusCode.Conflict), null);
                }
            }
            else
            {
                newFileName = DecodeFileName(_requestHeaders[WopiRequestHeader.SUGGESTED_TARGET]);

                if (newFileName.IndexOf('.') == 0) //have extention only
                {
                    newFileName = FileUtil.GetFileNameWithoutExtension(fileName) + newFileName;
                }

                if (FileNameIsExist(newFileName))
                {
                    newFileName = GetAvailableFileName(newFileName);
                }
            }

            return newFileName.Length > WopiFileHistoryConsts.MaxBaseFileNameLength ?  
                (new StatusCodeResult((int)HttpStatusCode.BadRequest), null) : 
                (new StatusCodeResult((int)HttpStatusCode.OK), newFileName);
        }

        private bool IsOverwriteRelativeTarget()
        {
            return _requestHeaders.ContainsKey(WopiRequestHeader.OVERWRITE_RELATIVE_TARGET) &&
                   _requestHeaders[WopiRequestHeader.OVERWRITE_RELATIVE_TARGET].ToString().ToLower() == "true";
        }

        private async Task ExtendLock(WopiFile file, string requestLock = null)
        {
            if (requestLock != null)
            {
                file.LockValue = requestLock;
            }

            file.LockExpires = DateTime.Now.AddMinutes(30);

            await _wopiFileRepository.UpdateAsync(file);
        }

        private async Task UpdateFileInfoInSubmission(Guid? submissionId, Guid fileId, UploadFileResult fileInfo, Guid? newFileId = null)
        {
            if (submissionId.HasValue)
            {
                fileInfo.FileId = newFileId == null ? fileId.ToString() : newFileId.ToString();
                await _formAppService.UpdateFileInfoInSubmission(submissionId.Value, fileId, fileInfo);
            }
        }

        #endregion
    }
}