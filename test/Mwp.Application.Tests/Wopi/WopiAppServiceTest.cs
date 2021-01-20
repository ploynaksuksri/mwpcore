using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Mwp.AzureStorage.TableEntities;
using Mwp.File;
using Mwp.Form;
using Mwp.Form.Dtos;
using Mwp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Xunit;

namespace Mwp.Wopi
{
    public class WopiAppServiceTest : MwpApplicationTestBase
    {
        private readonly IGuidGenerator GuidGenerator;
        private readonly ICurrentUser CurrentUser;
        private readonly IConfiguration Configuration;

        private readonly HttpContext _httpContext;
        private readonly IWopiAppService _wopiAppService;
        private readonly IFileAppService _fileAppService;
        private readonly IWopiFileHistoryRepository _wopiFileHistoryRepo;
        private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
        private readonly IWopiRequestAppService _wopiRequestAppService;
        private readonly IRepository<WopiFile> _wopiFileRepo;
        private readonly IFormAppService _formAppService;
        private readonly IRepository<Form.Form> _formRepo;

        private const string _accessToken =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijd2Vk1zTklTN2RTNnBEaXkya1NLZlEiLCJ0eXAiOiJhdCtqd3QifQ.eyJuYmYiOjE1OTEwNjkzMzEsImV4cCI6MTYyMjYwNTMzMSwiaXNzIjoiaHR0cHM6Ly9pZGVudGl0eXNlcnZlci1kZXYuZ28ubXl3b3JrcGFwZXJzLmNvLnVrIiwiYXVkIjoiTXdwIiwiY2xpZW50X2lkIjoiTXdwX0FwcCIsInN1YiI6IjU5OWE5OGQ1LTg0YjktNWIxNy0wYWNmLTM5ZjU2NmE1ZGYxNSIsImF1dGhfdGltZSI6MTU5MTA2OTMzMCwiaWRwIjoibG9jYWwiLCJyb2xlIjoiYWRtaW4iLCJuYW1lIjoiYWRtaW4iLCJlbWFpbCI6ImFkbWluQGFicC5pbyIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwic2NvcGUiOlsiTXdwIl0sImFtciI6WyJwd2QiXX0.bFv2W-T5vaMAmZEq-zKsgBHfiQmwL3v_h6moU_bRUWhafd9qxD0Fzs_7FM3dA7sQqRrqrjWChscNXY3JZdFy7hRsW5iU64KSWejK0Vqcohjwf5lYLZWwKIx9I-TXZtO0ve7cqV7yCNpcGtRxm9YHH82I11OZ3UBZC1W6EFjlbmhTSlipa791RyjprpSv10pzNRlyI9iiVxIH3AWVTK-frHkVkPb249Rf6N0skn3d9f7MaQZbA1INnPPwGJ2T7eF4456vkWWAHy4sj0Ea3ca0YO6Tddw5MHCODn5KdtCpw14I4vwObWzziXJW99hcjcISekqB2Z4qD0uyCVuwwNl28Q";

        public WopiAppServiceTest()
        {
            GuidGenerator = GetRequiredService<IGuidGenerator>();
            CurrentUser = GetRequiredService<ICurrentUser>();
            Configuration = GetRequiredService<IConfiguration>();

            var _httpContextAccessor = GetRequiredService<IHttpContextAccessor>();
            _httpContextAccessor.HttpContext = new DefaultHttpContext();
            _httpContext = _httpContextAccessor.HttpContext;
            SetRequestHeader("Authorization", $"Bearer {_accessToken}");

            _wopiFileRepo = GetRequiredService<IRepository<WopiFile>>();
            _wopiAppService = GetRequiredService<IWopiAppService>();
            _fileAppService = GetRequiredService<IFileAppService>();
            _wopiFileHistoryRepo = GetRequiredService<IWopiFileHistoryRepository>();
            _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
            _wopiRequestAppService = GetRequiredService<IWopiRequestAppService>();
            _formAppService = GetRequiredService<IFormAppService>();
            _formRepo = GetRequiredService<IRepository<Form.Form>>();
        }

        #region Tests for GetWopiActionUrl()

        [Fact]
        public async Task GetWopiActionUrl_WithNoHistoryId_And_FoundLatestHistoryRecord_Should_Success()
        {
            // arrange
            var fileName = "test.doc";
            var actionName = "view";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var wopiFileId = uploadedFileId;

            // act
            var result = await _wopiAppService.GetWopiActionUrl(wopiFileId, null, actionName);

            // assert
            result.FileId.ShouldBe(wopiFileId);
            result.FileIdInStorage.ShouldBe(uploadedFileId);
            result.BaseFileName.ShouldBe(fileName);
            result.AccessToken.ShouldBe(_accessToken);
            result.UrlSrc.ShouldContain($"wopisrc={Configuration["App:SelfUrl"]}/wopi/files/{wopiFileId}");
        }

        [Fact]
        public async Task GetWopiActionUrl_WithNoHistoryId_And_NotFoundLatestHistoryRecord_Should_ThrowEntityNotFoundException()
        {
            // arrange
            var fileName = "test.docx";
            var actionName = "embdedview";

            var uploadResult = await UploadFile(fileName);
            var wopiFileId = new Guid(uploadResult.FileId);

            await DeleteHistoryRecords(wopiFileId);

            // act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _wopiAppService.GetWopiActionUrl(wopiFileId, null, actionName)
            );
        }

        [Fact]
        public async Task GetWopiActionUrl_WithHistoryId_And_FoundTheHistoryRecord_Should_Success()
        {
            // arrange
            var fileName = "test.xlsx";
            var actionName = "edit";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var wopiFileId = uploadedFileId;

            var wopiFileHistory = await CreateWopiFileHistory(wopiFileId, 2, uploadResult); // reuse the uploaded file to create new history record

            // act
            var result = await _wopiAppService.GetWopiActionUrl(wopiFileId, wopiFileHistory.Id, actionName);

            // assert
            result.FileId.ShouldBe(wopiFileId);
            result.FileIdInStorage.ShouldBe(uploadedFileId);
            result.BaseFileName.ShouldBe(fileName);
            result.AccessToken.ShouldBe(_accessToken);
            result.UrlSrc.ShouldContain($"wopisrc={Configuration["App:SelfUrl"]}/wopi/files/{wopiFileId}_{wopiFileHistory.Id}");
        }

        [Fact]
        public async Task GetWopiActionUrl_WithHistoryId_And_NotFoundTheHistoryRecord_Should_ThrowEntityNotFoundException()
        {
            // arrange
            var fileName = "test.xlsx";
            var actionName = "edit";

            var uploadResult = await UploadFile(fileName);
            var wopiFileId = new Guid(uploadResult.FileId);

            // act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _wopiAppService.GetWopiActionUrl(wopiFileId, GuidGenerator.Create(), actionName)
            );
        }

        [Fact]
        public async Task GetWopiActionUrl_NotSupportActionName_Should_ThrowInvalidOperationException()
        {
            // arrange
            var fileName = "test.doc";
            var actionName = "edit"; // not support edit action for lagacy file (.doc, .xls, .ppt)

            var uploadResult = await UploadFile(fileName);
            var wopiFileId = new Guid(uploadResult.FileId);

            // act and assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _wopiAppService.GetWopiActionUrl(wopiFileId, null, actionName)
            );
        }

        #endregion Tests for GetWopiActionUrl()

        #region Tests for GetWopiFileHistories()

        [Fact]
        public async Task GetWopiFileHistories_Should_Success()
        {
            // arrange
            var fileName = "test.doc";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var wopiFileId = uploadedFileId;

            var wopiFileHistory = await CreateWopiFileHistory(wopiFileId, 2, uploadResult); // reuse the uploaded file to create new history record

            // act
            var result = await _wopiAppService.GetWopiFileHistories(wopiFileId);

            // assert
            var histories = result.FileHistories;
            histories.Count.ShouldBe(2);
            histories[0].Id.ShouldBe(wopiFileHistory.Id); // order by creation date DESC
            histories[0].LastModificationDetail.ShouldBe($"Update file {uploadResult.FileName}");
            histories[0].LastModificationTime.ShouldNotBeNull();
            histories[1].LastModificationTime.ShouldNotBeNull();
            histories[1].LastModificationTime.Value.ShouldBeLessThan(histories[0].LastModificationTime.Value);
            histories[1].LastModificationDetail.ShouldBe($"Uploaded file {uploadResult.FileName}");
        }

        #endregion Tests for GetWopiFileHistories()

        #region Test for Replace and Checkout/Checkin

        [Fact]
        public async Task CheckFileInfo_WhenFileIsCheckedout_ShouldReturn_FileInfoWithReadOnlyPermission()
        {
            // arrange
            var uploadFileResult = await UploadFile("test.doc");
            var fileId = new Guid(uploadFileResult.FileId);
            WopiFileHistory wopiFileHistory = null;
            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(Guid.NewGuid(), "1"))
                {
                    wopiFileHistory = await _wopiFileHistoryRepo.GetLatestHistory(fileId);
                    await _wopiAppService.CheckoutFile(fileId);
                }
            });
            wopiFileHistory.ShouldNotBeNull();

            // act
            IActionResult result;
            using (ChangeCurrentUser(Guid.NewGuid(), "2"))
            {
                result = await _wopiRequestAppService.ProcessWopiRequest($"{fileId}_{wopiFileHistory.Id}", WopiRequestType.CheckFileInfo, false);
            }

            // assert
            var output = result.ShouldBeOfType<ContentResult>();
            output.StatusCode.ShouldBe((int)HttpStatusCode.OK);
            var content = JsonConvert.DeserializeObject<WopiFileDto>(output.Content);

            content.UserCanWrite.ShouldBeFalse();
            content.UserCanRename.ShouldBeFalse();
            content.SupportsDeleteFile.ShouldBeFalse();
            content.SupportsRename.ShouldBeFalse();
            content.UserCanNotWriteRelative.ShouldBeTrue();
        }

        [Fact]
        public async Task ReplaceFile_Should_Success()
        {
            // arrange
            var fileName = "test.doc";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD V2");
            await WithUnitOfWorkAsync(async () =>
            {
                await _wopiAppService.ReplaceFile(uploadedFileId, data);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var latestFileHistory = await _wopiFileHistoryRepo.GetLatestHistory(uploadedFileId);
                latestFileHistory.ShouldNotBeNull();
                var latestFileId = latestFileHistory.FileIdInStorage;
                var latestFile = await _fileAppService.DownloadFile(latestFileId.ToString());
                latestFile.ShouldNotBeNull();
                var latestContent = latestFile.Data;
                var latestString = Encoding.UTF8.GetString(latestContent);
                latestString.ShouldBe("TEST DATA UPLOAD V2");
            });
        }


        [Fact]
        public async Task CheckoutFile_Should_Success()
        {
            // arrange
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var uploadedFileId = new Guid(uploadResult.FileId);

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(Guid.NewGuid(), "1"))
                {
                    await _wopiAppService.CheckoutFile(uploadedFileId);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var latestFileHistory = await _wopiFileHistoryRepo.GetLatestHistory(uploadedFileId);
                var wopiFile = await _wopiFileRepo.GetAsync(x => x.Id == latestFileHistory.WopiFileId);
                wopiFile.ShouldNotBeNull();
                wopiFile.CheckoutBy.ShouldNotBeNull();
                wopiFile.CheckoutTimestamp.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task CheckoutFile_Should_PreventFileReplaceFromOtherUser()
        {
            // arrange
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var uploadedFileId = new Guid(uploadResult.FileId);

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(Guid.NewGuid(), "1"))
                {
                    await _wopiAppService.CheckoutFile(uploadedFileId);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(Guid.NewGuid(), "2"))
                {
                    var newData = Encoding.UTF8.GetBytes("TEST DATA UPLOAD V2");
                    var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
                    {
                        await _wopiAppService.ReplaceFile(uploadedFileId, newData);
                    });
                    exception.Message.ShouldBe("File was checked-out by another.");
                }
            });
        }

        [Fact]
        public async Task CheckoutFile_Should_NotPreventFileReplaceForSameUser()
        {
            // arrange
            var userId = Guid.NewGuid();
            var usernameSuffix = "1";
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var uploadedFileId = new Guid(uploadResult.FileId);

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    await _wopiAppService.CheckoutFile(uploadedFileId);
                }
            });
            WopiFileHistory historyBeforeChange = null;

            await WithUnitOfWorkAsync(async () =>
            {
                historyBeforeChange = await GetLatestHistory(uploadedFileId);
                historyBeforeChange.ShouldNotBeNull();
            });

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    var newData = Encoding.UTF8.GetBytes("TEST DATA UPLOAD V2");
                    await _wopiAppService.ReplaceFile(uploadedFileId, newData);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var historyAfterReplace = await GetLatestHistory(uploadedFileId);
                historyAfterReplace.ShouldNotBeNull();
                historyAfterReplace.Id.ShouldNotBe(historyBeforeChange.Id);
                historyAfterReplace.Version.ShouldBeGreaterThan(historyBeforeChange.Version);
            });
        }

        [Fact]
        public async Task CheckinFile_Should_Success()
        {
            // arrange
            var userId = Guid.NewGuid();
            var usernameSuffix = "1";
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var uploadedFileId = new Guid(uploadResult.FileId);

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    await _wopiAppService.CheckoutFile(uploadedFileId);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var file = await _wopiFileRepo.FindAsync(x => x.Id == uploadedFileId);
                file.CheckoutBy.ShouldNotBeNull();
                file.CheckoutTimestamp.ShouldNotBeNull();
            });

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    await _wopiAppService.CheckinFile(uploadedFileId);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var wopiFile = await _wopiFileRepo.GetAsync(x => x.Id == uploadedFileId);
                wopiFile.ShouldNotBeNull();
                wopiFile.CheckoutBy.ShouldBeNull();
                wopiFile.CheckoutTimestamp.ShouldBeNull();
            });
        }

        [Fact]
        public async Task CheckinAndReplaceFile_Should_Success()
        {
            // arrange
            var userId = Guid.NewGuid();
            var usernameSuffix = "1";
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var uploadedFileId = new Guid(uploadResult.FileId);

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    await _wopiAppService.CheckoutFile(uploadedFileId);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                using (ChangeCurrentUser(userId, usernameSuffix))
                {
                    var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD V2");
                    await _wopiAppService.CheckinAndReplaceFile(uploadedFileId, data);
                }
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var latestFileHistory = await _wopiFileHistoryRepo.GetLatestHistory(uploadedFileId);
                var wopiFile = await _wopiFileRepo.GetAsync(x => x.Id == latestFileHistory.WopiFileId);
                wopiFile.ShouldNotBeNull();
                wopiFile.CheckoutBy.ShouldBeNull();
                wopiFile.CheckoutTimestamp.ShouldBeNull();

                var latestFile = await _fileAppService.DownloadFile(latestFileHistory.FileIdInStorage.ToString());
                latestFile.ShouldNotBeNull();
                var latestContent = latestFile.Data;
                var latestString = Encoding.UTF8.GetString(latestContent);
                latestString.ShouldBe("TEST DATA UPLOAD V2");
            });
        }

        #endregion Test for Replace and Checkout/Checkin

        #region Tests for RestoreToHistory()

        [Fact]
        public async Task RestoreToHistory_WithValidHistoryId_Should_Success()
        {
            // arrange
            var fileName = "test.doc";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var wopiFileId = uploadedFileId;
            var firstHistory = await GetLatestHistory(wopiFileId);

            await CreateWopiFileHistory(wopiFileId, 2, uploadResult); // reuse the uploaded file to create new history record

            // act
            var result = await _wopiAppService.RestoreToHistory(wopiFileId, firstHistory.Id);

            // assert
            var restoredHistoryRecord = await GetHistory(result);
            restoredHistoryRecord.WopiFileId.ShouldBe(wopiFileId);
            restoredHistoryRecord.Version.ShouldBe(3); // version number increased from the lastest version
            restoredHistoryRecord.LastModificationTime.Value.ShouldBeGreaterThan(firstHistory.LastModificationTime.Value);
            restoredHistoryRecord.LastModificationDetail.ShouldBe($"Restored to version {firstHistory.LastModificationTime.Value:MMM dd, yyyy h:mm tt}");
            restoredHistoryRecord.WopiFileId.ShouldBe(wopiFileId);
        }

        [Fact]
        public async Task RestoreToHistory_WithInvalidHistoryId_Should_ThrowEntityNotFoundException()
        {
            // arrange
            var fileName = "test.doc";

            var uploadResult = await UploadFile(fileName);

            var uploadedFileId = new Guid(uploadResult.FileId);
            var wopiFileId = uploadedFileId;

            // act and assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _wopiAppService.RestoreToHistory(wopiFileId, GuidGenerator.Create())
            );
        }

        #endregion Tests for RestoreToHistory()

        #region Test for Form's Document

        [Fact]
        public async Task WhenSaveFormWithDefaultFileForFileUploader_Should_CreateFileInStorageWithReferrerAsForm()
        {
            var tuple = await CreateFormWithDefaultFile();
            var savedForm = tuple.Item1;
            var uploadResult = tuple.Item2;
            await WithUnitOfWorkAsync(async () =>
            {
                var fileInfo = await _fileAppService.GetFileInfo(uploadResult.FileId);
                fileInfo.ShouldNotBeNull();
                var fileReferrerId = fileInfo["ReferredBy"]?.Value<string>();
                fileReferrerId.ShouldNotBeNull();
                fileReferrerId.ShouldBe(savedForm["_id"]?.Value<string>());
                var fileReferrerType = fileInfo["ReferrerType"]?.Value<string>();
                fileReferrerType.ShouldNotBeNull();
                fileReferrerType.ShouldBe("Form");
            });
        }


        [Fact]
        public async Task WhenSaveFormWithDefaultFileForFileUploader_Should_CreateWopiFileThatReferenceToForm()
        {
            var tuple = await CreateFormWithDefaultFile();
            var savedForm = tuple.Item1;
            var formId = new Guid(savedForm["_id"]!.Value<string>());
            var uploadResult = tuple.Item2;
            await WithUnitOfWorkAsync(async () =>
            {
                var wopiFile = await _wopiFileRepo.FindAsync(x => x.Id == new Guid(uploadResult.FileId));
                wopiFile.ShouldNotBeNull();
                wopiFile.FormId.ShouldNotBeNull();
                wopiFile.FormId.ShouldBe(formId);
                wopiFile.SubmissionId.ShouldBeNull();
            });
        }

        [Fact]
        public async Task WhenSaveSubmissionOfFormWithDefaultFileForFileUploader_Should_CreateWopiFileThatReferenceToSubmission()
        {
            var tuple = await CreateFormWithDefaultFile();
            var savedForm = tuple.Item1;
            var formId = new Guid(savedForm["_id"]!.Value<string>());
            JToken saveSubmissionJObj = null;
            await WithUnitOfWorkAsync(async () =>
            {
                var submissionJson = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                    typeof(FormAppServiceTest).Assembly,
                    "Mwp.Wopi.json.submission-with-default-file-from-form.json");
                var submissionJObj = JObject.Parse(submissionJson);
                var dataJObj = submissionJObj.Value<JObject>(FormIoProps.Data);
                var files = dataJObj.Value<JArray>("files");
                files.Count.ShouldBe(1);
                var file = files[0];
                var uploadResult = tuple.Item2;
                file[FormIoProps.FileProps.FileId] = uploadResult.FileId;
                var saveSubmissionJson = await _formAppService.SaveSubmission(formId, submissionJObj);
                saveSubmissionJObj = JObject.Parse(saveSubmissionJson);
            });

            saveSubmissionJObj.ShouldNotBeNull();

            await WithUnitOfWorkAsync(async () =>
            {
                var submissionId = new Guid(saveSubmissionJObj["_id"]!.Value<string>());
                var wopiFile = await _wopiFileRepo.FindAsync(x => x.SubmissionId == submissionId);
                wopiFile.ShouldNotBeNull();
                wopiFile.SubmissionId.ShouldNotBeNull();
                wopiFile.SubmissionId.ShouldBe(submissionId);

                var fileInfo = await _fileAppService.GetFileInfo(wopiFile.Id.ToString());
                fileInfo.ShouldNotBeNull();
                fileInfo[nameof(UploadFileTableEntity.ReferrerType)]!.Value<string>().ShouldBe(nameof(Submission));
                fileInfo[nameof(UploadFileTableEntity.ReferredBy)]!.Value<string>().ShouldBe(submissionId.ToString());
            });
        }

        [Fact]
        public async Task WhenRolloverFormWithDefaultFileForFileUploaderButItIsNotSetToRollOverDocuments_Should_NotCreateAnyCopiedFileInStorage()
        {
            var tuple = await CreateFormWithDefaultFile();
            var savedForm = tuple.Item1;
            var formId = new Guid(savedForm["_id"]!.Value<string>());
            await WithUnitOfWorkAsync(async () =>
            {
                var rolloverDto = new FormRolloverInfoDto
                {
                    FormId = formId,
                    IsPreserveParentChanges = true,
                    NewFormName = "NewTestFormWithDefaultFile"
                };
                var rolloverResult = await _formAppService.RolloverForm(rolloverDto);
                rolloverResult.ShouldBe(true);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var childForm = await _formRepo.FindAsync(x => x.ParentId == formId);
                childForm.ShouldNotBeNull();

                var childFormJObj = JObject.Parse(childForm.Data);
                var childFormComponents = childFormJObj["components"] as JArray;
                childFormComponents.ShouldNotBeNull();
                var childFormFileUploader = childFormComponents.FirstOrDefault(x => x["key"]!.Value<string>() == "files");
                childFormFileUploader.ShouldNotBeNull();
                childFormFileUploader["mwp"].ShouldNotBeNull();
                var documents = childFormFileUploader["mwp"]["documents"] as JArray;
                documents.ShouldNotBeNull();
                documents.Count.ShouldBe(0);
            });
        }

        [Fact]
        public async Task WhenRolloverFormWithDefaultFileForFileUploader_Should_CreateCopiedFileInStorage()
        {
            var tuple = await CreateFormWithDefaultFile(true);
            var savedForm = tuple.Item1;
            var formId = new Guid(savedForm["_id"]!.Value<string>());
            var uploadResult = tuple.Item2;
            await WithUnitOfWorkAsync(async () =>
            {
                var rolloverDto = new FormRolloverInfoDto
                {
                    FormId = formId,
                    IsPreserveParentChanges = true,
                    NewFormName = "NewTestFormWithDefaultFile"
                };
                var rolloverResult = await _formAppService.RolloverForm(rolloverDto);
                rolloverResult.ShouldBe(true);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var childForm = await _formRepo.FindAsync(x => x.ParentId == formId);
                childForm.ShouldNotBeNull();

                var childFormJObj = JObject.Parse(childForm.Data);
                var childFormComponents = childFormJObj["components"] as JArray;
                childFormComponents.ShouldNotBeNull();
                var childFormFileUploader = childFormComponents.FirstOrDefault(x => x["key"]!.Value<string>() == "files");
                childFormFileUploader.ShouldNotBeNull();
                childFormFileUploader["mwp"].ShouldNotBeNull();
                var documents = childFormFileUploader["mwp"]["documents"] as JArray;
                documents.ShouldNotBeNull();
                documents.Count.ShouldBe(1);
                var file0 = documents[0];
                file0.ShouldNotBeNull();
                file0["fileHash"]!.Value<string>().ShouldBe(uploadResult.FileHash);
                file0["fileName"]!.Value<string>().ShouldBe(uploadResult.FileName);
                file0["fileSize"]!.Value<int>().ShouldBe(uploadResult.FileSize);
                file0["fileId"]!.Value<string>().ShouldNotBe(uploadResult.FileId);

                var file0Id = new Guid(file0["fileId"]!.Value<string>());

                var file0InStorage = await _fileAppService.GetFileInfo(file0Id.ToString());
                file0InStorage.ShouldNotBeNull();

                var childWopiFile = await _wopiFileRepo.FindAsync(x => x.Id == file0Id);
                childWopiFile.ShouldNotBeNull();
                childWopiFile.FormId.ShouldBe(childForm.Id);
                childWopiFile.SubmissionId.ShouldBeNull();
            });
        }

        #endregion

        #region Private Methods

        private async Task<Tuple<JToken, UploadFileResult>> CreateFormWithDefaultFile(bool includeDocInRollOver = false)
        {
            var fileName = "test.doc";
            var uploadResult = await UploadFile(fileName);
            var formJson = MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(
                typeof(FormAppServiceTest).Assembly,
                "Mwp.Wopi.json.form-with-default-file-template.json");
            var formJObj = JObject.Parse(formJson);
            var components = formJObj["components"] as JArray;
            components.ShouldNotBeNull();
            var fileUploader = components.Children().FirstOrDefault(x => x.Value<string>("key") == "files");
            fileUploader.ShouldNotBeNull();
            fileUploader!["mwp"]!["includeInTheRollOver"] = true;
            if (includeDocInRollOver)
            {
                fileUploader!["mwp"]!["includeDocumentsInTheRollOver"] = true;
            }
            var documents = fileUploader["mwp"]?["documents"] as JArray;
            documents.ShouldNotBeNull();
            var file = documents[0];
            file.ShouldNotBeNull();
            file["fileId"] = uploadResult.FileId;
            file["fileName"] = uploadResult.FileName;
            file["fileSize"] = uploadResult.FileSize;
            file["fileHash"] = uploadResult.FileHash;
            JToken savedForm = null;
            await WithUnitOfWorkAsync(async () =>
            {
                var savedFormJson = await _formAppService.SaveForm(formJObj);
                savedFormJson.ShouldNotBeNull();
                savedForm = JToken.Parse(savedFormJson);
                savedForm.ShouldNotBeNull();
                savedForm["_id"]?.Value<string>().ShouldNotBeNull();
            });
            return new Tuple<JToken, UploadFileResult>(savedForm, uploadResult);
        }

        private void SetRequestHeader(string header, string value)
        {
            _httpContext.Request.Headers[header] = value;
        }

        private async Task<UploadFileResult> UploadFile(string fileName)
        {
            var data = Encoding.UTF8.GetBytes("TEST DATA UPLOAD");
            var uploadResult = await _fileAppService.UploadFile(data, FileUtil.GetMimeMapping(fileName), fileName, data.Length);
            uploadResult.ShouldNotBeNull();
            uploadResult.FileId.ShouldNotBeNullOrEmpty();

            return uploadResult;
        }

        private async Task<WopiFileHistory> CreateWopiFileHistory(Guid wopiFileId, int versionNumber, UploadFileResult uploadResult)
        {
            var newFileHistory = new WopiFileHistory
            {
                WopiFileId = wopiFileId,
                Version = versionNumber,
                Revision = 0,
                Size = uploadResult.FileSize,
                FileIdInStorage = new Guid(uploadResult.FileId),
                BaseFileName = uploadResult.FileName,
                LastModificationTime = DateTime.Now,
                LastModificationDetail = $"Update file {uploadResult.FileName}",
                LastModifiedUsers = CurrentUser.Id.ToString()
            };

            return await _wopiFileHistoryRepo.InsertAsync(newFileHistory, true);
        }

        private async Task DeleteHistoryRecords(Guid wopiFileId)
        {
            await _wopiFileHistoryRepo.DeleteAsync(h => h.WopiFileId == wopiFileId, true);
        }

        private Task<WopiFileHistory> GetLatestHistory(Guid wopiFileId)
        {
            return _wopiFileHistoryRepo.GetLatestHistory(wopiFileId);
        }

        private Task<WopiFileHistory> GetHistory(Guid historyId)
        {
            return _wopiFileHistoryRepo.GetAsync(h => h.Id == historyId);
        }

        private IDisposable ChangeCurrentUser(Guid userId, string username)
        {
            var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(AbpClaimTypes.UserId, userId.ToString()),
                new Claim(AbpClaimTypes.UserName, $"TestUser{username}")
            }));

            return _currentPrincipalAccessor.Change(newPrincipal);
        }

        #endregion Private Methods
    }
}