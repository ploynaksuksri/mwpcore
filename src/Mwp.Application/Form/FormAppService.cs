using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Mwp.ExcelIntegration.Form;
using Mwp.File;
using Mwp.File.Events;
using Mwp.Form.Dtos;
using Mwp.Form.Events;
using Mwp.Form.Repository;
using Mwp.Permissions;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;

namespace Mwp.Form
{
    [RemoteService(false)]
    [Authorize(MwpPermissions.Form.Default)]
    public class FormAppService : MwpAppService, IFormAppService
    {
        public static readonly string[] FileUploadComponentNames = { "mwp-fileupload", "mwp-ng-file-uploader" };
        private readonly ILocalEventBus _eventBus;
        private readonly IFormRepository _formRepository;
        private readonly IFormStorageClient _formsStorageClient;
        private readonly IRepository<Submission> _submissionRepository;
        private readonly IRepository<UserFormConfig> _userFormConfigRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IFileStorageClient _fileStorageClient;

        public FormAppService(
            IRepository<Submission> submissionRepository,
            IRepository<UserFormConfig> userFormConfigRepository,
            IFormRepository formRepository,
            IFormStorageClient formsStorageClient,
            IFileStorageClient fileStorageClient,
            ILocalEventBus eventBus,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _formsStorageClient = formsStorageClient;
            _userFormConfigRepository = userFormConfigRepository;
            _formRepository = formRepository;
            _submissionRepository = submissionRepository;
            _eventBus = eventBus;
            _fileStorageClient = fileStorageClient;
        }

        public async Task<string> GetFormByIdAsJsonAsync(Guid formId)
        {
            var form = await _formRepository.GetFormByIdAsync(formId);
            return form?.TenantId != CurrentTenant?.Id ? null : form?.Data;
        }

        public async Task<string> GetSubmissionByIdAsJsonAsync(Guid formId, Guid submissionId)
        {
            var submission = await _submissionRepository.FindAsync(x => x.Id == submissionId);
            return submission?.TenantId != CurrentTenant?.Id ? null : submission?.Data;
        }

        public async Task<string> SaveSubmission(Guid formId, JToken token)
        {
            if (token == null)
            {
                return null;
            }

            var id = string.IsNullOrEmpty(token.Value<string>(FormIoProps.ObjId))
                ? Guid.Empty
                : new Guid(token.Value<string>(FormIoProps.ObjId));
            var (submission, form) = await GetExistSubmission(id, formId);
            if (form == null)
            {
                throw new Exception("Form not found.");
            }

            var isNew = false;
            var isModified = false;
            if (submission == null)
            {
                isNew = true;
                var metaJObj = token.Value<JObject>(FormIoProps.MwpMetaData);
                if (metaJObj == null)
                {
                    metaJObj = new JObject
                    {
                        { FormIoProps.CurrentVersion, GuidGenerator.Create() }
                    };
                    token[FormIoProps.MwpMetaData] = metaJObj;
                }
                submission = CreateNewSubmission(token, form);
                submission.TenantId = CurrentTenant?.Id;
            }
            else
            {
                var existSubmissionJObj = JObject.Parse(submission.Data);
                var metaJObj = existSubmissionJObj.Value<JObject>(FormIoProps.MwpMetaData);
                if (metaJObj == null)
                {
                    metaJObj = new JObject();
                    token[FormIoProps.MwpMetaData] = metaJObj;
                }
                metaJObj[FormIoProps.CurrentVersion] = GuidGenerator.Create();
                token[FormIoProps.MwpMetaData] = metaJObj;
                isModified = UpdateSubmission(submission, token);
            }

            await AppendExternalIdsToChildrenSubmissions(submission, form);
            await ReplaceAllFilesFromTemplateWithCopiedFile(submission, form);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            var submissionJObj = JObject.Parse(submission.Data);
            UpdateParentNodeIfSubmissionContainMergeData(submissionJObj, token[FormIoProps.MergeData]);
            submission.Data = submissionJObj.ToString();
            await SaveSubmission(isNew, isModified, submission);
            await AttachReferrerIdToFilesInSubmission(submission, form);
            return submission.Data;
        }

        async Task ReplaceAllFilesFromTemplateWithCopiedFile(Submission submission, Form form)
        {
            var subJObj = JObject.Parse(submission.Data);
            var formJObj = JObject.Parse(form.Data);
            var fileUploadTokens = ExtractAllFileUploadTokensInForm(formJObj);
            foreach (var fileUploadToken in fileUploadTokens)
            {
                var key = fileUploadToken.Value<string>(FormIoProps.Key);
                var fileTokens = subJObj.SelectTokens("$.data." + key + "[?(@isFromTemplate==true)]").ToList();
                foreach (var fileToken in fileTokens)
                {
                    var originalFileId = fileToken.Value<string>(FormIoProps.FileProps.FileId);
                    var copiedResult = await _fileStorageClient.CopyFileById(originalFileId);
                    await _eventBus.PublishAsync(new UploadedFileEventData(copiedResult));
                    fileToken[FormIoProps.FileProps.FileId] = copiedResult.FileId;
                    fileToken[FormIoProps.FileProps.IsFromTemplate] = false;
                }
            }
            submission.Data = subJObj.ToString();
        }

        private async Task AttachReferrerIdToFilesInSubmission(Submission submission, Form form)
        {
            var fileIds = GetAllFileIdsInSubmission(submission, form);
            if (fileIds == null || fileIds.Length <= 0)
            {
                return;
            }

            await ClearReferrerIdOfExistFiles(submission.Id);
            await AssignReferrerIdToFiles(submission.Id, fileIds, FileReferrerType.Submission);
        }

        private JToken[] ExtractAllFileUploadTokensInForm(JObject formJObject)
        {
            var fileUploadTokens = new List<JToken>();
            foreach (var fileUploadComponentName in FileUploadComponentNames)
            {
                fileUploadTokens.AddRange(ExtractAllComponentsInFormWithType(formJObject, fileUploadComponentName));
            }

            return fileUploadTokens.ToArray();
        }

        protected virtual string[] GetAllFileIdsInSubmission(Submission submission, Form form)
        {
            if (form == null || submission == null || string.IsNullOrEmpty(form.Data) || string.IsNullOrEmpty(submission.Data))
            {
                return new string[0];
            }

            var fileIds = new List<string>();
            var subJObj = JObject.Parse(submission.Data);
            var formJObj = JObject.Parse(form.Data);
            var fileUploadTokens = ExtractAllFileUploadTokensInForm(formJObj);
            foreach (var fileUploadToken in fileUploadTokens)
            {
                var key = fileUploadToken.Value<string>("key");
                var fileIdTokens = subJObj.SelectTokens("$.data." + key + "..fileId").ToList();
                if (fileIdTokens.Any())
                {
                    fileIds.AddRange(fileIdTokens.Select(x => x.Value<string>()));
                }
            }

            return fileIds.ToArray();
        }

        private JToken[] ExtractAllComponentsInFormWithType(JObject formJObj, string type)
        {
            if (formJObj == null)
            {
                return new JToken[0];
            }

            var tokens = formJObj.SelectTokens("$..components[?(@.type=='" + type + "')]");
            return tokens.ToArray();
        }

        public async Task<string> SaveForm(JToken token)
        {
            if (token == null)
            {
                return null;
            }

            var isNew = false;
            var form = await GetFormForFormToken(token);

            if (form == null)
            {
                form = new Form(GuidGenerator.Create());
                isNew = true;
                token[FormIoProps.ObjId] = form.Id.ToString();
            }

            form.TenantId = CurrentTenant?.Id;
            form.Name = token[FormIoProps.Name]!.Value<string>();
            AppendMwpFormMetaData(form, token);
            UpdateParentNodeIfFormContainMergeData(token, token[FormIoProps.MergeData]);
            AssignHierarchicalPath(form, token);
            form.CurrentVersion = GuidGenerator.Create();
            var metaData = token[FormIoProps.MwpMetaData]?.Value<JObject>();
            if (metaData != null)
            {
                metaData![FormIoProps.CurrentVersion] = form.CurrentVersion.ToString();
            }
            form.Data = token.ToString();
            await SaveFormAsync(isNew, form);
            var formJObj = JObject.Parse(form.Data);
            await AttachFormIdToFilesInForm(formJObj);
            return form.Data;
        }

        async Task SaveFormAsync(bool isNew, Form form)
        {
            if (isNew)
            {
                await _formRepository.InsertAsync(form);
            }
            else
            {
                await _formRepository.UpdateAsync(form);
            }
        }

        void UpdateParentNodeIfFormContainMergeData(JToken token, JToken mergeData)
        {
            var mergeParentFormId = mergeData?[FormIoProps.MwpProps.ParentFormId]?.Value<string>();
            if (mergeParentFormId == null)
            {
                return;
            }

            var metaData = token[FormIoProps.MwpMetaData]?.Value<JObject>();
            var parentNodes = metaData?[FormIoProps.MwpProps.ParentNodes]?.Value<JArray>();
            if (metaData == null)
            {
                metaData = new JObject();
                token[FormIoProps.MwpMetaData] = metaData;
            }

            if (parentNodes == null)
            {
                parentNodes = new JArray();
                metaData.Add(FormIoProps.MwpProps.ParentNodes, parentNodes);
            }
            var mergeParentFormVersion = mergeData[FormIoProps.MwpProps.ParentFormVersion]?.Value<string>();
            var parentNode = parentNodes.Children().FirstOrDefault(x => x[FormIoProps.ParentNodeProps.Id]?.Value<string>() == mergeData[FormIoProps.MwpProps.ParentFormId]?.Value<string>());
            if (parentNode == null)
            {
                parentNode = new JObject { [FormIoProps.ParentNodeProps.Id] = mergeParentFormId };
                parentNodes.Add(parentNode);
            }
            parentNode[FormIoProps.ParentNodeProps.Version] = mergeParentFormVersion;
            var jobj = token as JObject;
            if (jobj?[FormIoProps.MergeData] != null)
            {
                jobj.Remove(FormIoProps.MergeData);
            }
        }

        void UpdateParentNodeIfSubmissionContainMergeData(JToken token, JToken mergeData)
        {
            var mergeParentFormId = mergeData?[FormIoProps.MwpProps.ParentSubmissionId]?.Value<string>();
            if (mergeParentFormId == null)
            {
                return;
            }

            var metaData = token[FormIoProps.MwpMetaData]?.Value<JObject>();
            var parentNodes = metaData?[FormIoProps.MwpProps.ParentNodes]?.Value<JArray>();
            if (metaData == null)
            {
                metaData = new JObject();
                token[FormIoProps.MwpMetaData] = metaData;
            }

            if (parentNodes == null)
            {
                parentNodes = new JArray();
                metaData.Add(FormIoProps.MwpProps.ParentNodes, parentNodes);
            }

            var mergeParentFormVersion = mergeData[FormIoProps.MwpProps.ParentSubmissionVersion]?.Value<string>();
            var parentNode = parentNodes.Children().FirstOrDefault(x => x[FormIoProps.ParentNodeProps.Id]?.Value<string>() == mergeData[FormIoProps.MwpProps.ParentSubmissionId]?.Value<string>());
            if (parentNode == null)
            {
                parentNode = new JObject { [FormIoProps.ParentNodeProps.Id] = mergeParentFormId };
                parentNodes.Add(parentNode);
            }
            parentNode[FormIoProps.ParentNodeProps.Version] = mergeParentFormVersion;
            var jobj = token as JObject;
            if (jobj?[FormIoProps.MergeData] != null)
            {
                jobj.Remove(FormIoProps.MergeData);
            }
        }

        private async Task AttachFormIdToFilesInForm(JObject formJObj)
        {
            var formId = new Guid(formJObj["_id"]!.Value<string>());
            var fileIds = GetAllFileIdsInForm(formJObj);
            if (fileIds == null || fileIds.Length <= 0)
            {
                return;
            }

            await ClearReferrerIdOfExistFiles(formId);
            await AssignReferrerIdToFiles(formId, fileIds, FileReferrerType.Form);
        }

        public string[] GetAllFileIdsInForm(JObject formJObj)
        {
            var fileIds = new List<string>();

            var fileUploadTokens = ExtractAllFileUploadTokensInForm(formJObj);
            foreach (var fileUploadToken in fileUploadTokens)
            {
                var fileIdTokens = fileUploadToken.SelectTokens("$.mwp.documents..fileId").ToList();
                if (fileIdTokens.Any())
                {
                    fileIds.AddRange(fileIdTokens.Select(x => x.Value<string>()));
                }
            }

            return fileIds.ToArray();
        }

        public async Task<JArray> GetParentFormsByFormId(Guid formId)
        {
            var parentFormJArr = new JArray();
            var form = await _formRepository.GetFormByIdAsync(formId);
            if (form == null || string.IsNullOrEmpty(form.HierarchicalPath))
            {
                return parentFormJArr;
            }

            var parentIdStrs = form.HierarchicalPath.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (parentIdStrs.Length <= 0)
            {
                return parentFormJArr;
            }
            var parentFormIds = parentIdStrs.Select(x => new Guid(x)).ToList();
            var parentForms = _formRepository.Where(f => parentFormIds.Contains(f.Id)).ToList();
            foreach (var parentForm in parentForms)
            {
                var parentFormJObj = JObject.Parse(parentForm.Data);
                parentFormJObj.Add(FormIoProps.CurrentVersion, parentForm.CurrentVersion);
                parentFormJArr.Add(parentFormJObj);
            }
            return parentFormJArr;
        }

        private void AssignHierarchicalPath(Form form, JToken token)
        {
            var idJArr = token[FormIoProps.MwpMetaData]?[FormIoProps.MwpProps.HierarchicalPath]?.Value<JArray>();
            if (idJArr == null || idJArr.Count <= 0)
            {
                return;
            }

            var ids = idJArr.ToObject<List<string>>();
            var pathStr = string.Join("/", ids!);
            form.HierarchicalPath = pathStr;
            form.ParentId = new Guid(ids.Last());
        }

        public async Task<string> ListForm(string searchText, long skip, long take)
        {
            var token = await _formRepository.ListForm(CurrentTenant?.Id, searchText, skip, take);
            return token?.ToString();
        }

        public async Task<string> ListSubmission(Guid formId, long skip, long take)
        {
            var token = await _formRepository.ListSubmission(CurrentTenant?.Id, formId, skip, take);
            return token?.ToString();
        }

        public async Task<JArray> GetFormsForLookUp(long limit, string[] columns)
        {
            return await _formRepository.GetFormsForLookUp(CurrentTenant?.Id, limit, columns);
        }

        public async Task<JToken> SaveFormBuilderConfiguration(Guid userId, JToken formBuilderConfig)
        {
            var userFormConfig = await _userFormConfigRepository.FindAsync(x => x.Id == userId);
            if (formBuilderConfig[FormIoProps.UserId] == null)
            {
                formBuilderConfig[FormIoProps.UserId] = userId;
            }

            if (userFormConfig == null)
            {
                userFormConfig = new UserFormConfig(userId)
                {
                    FormBuilderConfig = formBuilderConfig.ToString()
                };
                userFormConfig.TenantId = CurrentTenant?.Id;
                await _userFormConfigRepository.InsertAsync(userFormConfig);
            }
            else
            {
                userFormConfig.FormBuilderConfig = formBuilderConfig.ToString();
                await _userFormConfigRepository.UpdateAsync(userFormConfig);
            }

            return formBuilderConfig;
        }

        public async Task<JToken> GetFormBuilderConfiguration(Guid userId)
        {
            var userFormConfig = await _userFormConfigRepository.FindAsync(x => x.Id == userId);
            return string.IsNullOrEmpty(userFormConfig?.FormBuilderConfig) ? null : JObject.Parse(userFormConfig.FormBuilderConfig);
        }

        public async Task<JObject> GetFormHistory(Guid formId, string rowKey)
        {
            return await _formsStorageClient.GetFormHistory(formId, rowKey);
        }

        public async Task<JObject> GetSubmissionHistory(Guid submissionId, string rowKey)
        {
            return await _formsStorageClient.GetSubmissionHistory(submissionId, rowKey);
        }

        public async Task<JArray> ListFormHistory(Guid formId)
        {
            return await _formsStorageClient.ListFormHistory(formId);
        }

        public async Task<JArray> ListSubmissionHistory(Guid submissionId)
        {
            return await _formsStorageClient.ListSubmissionHistory(submissionId);
        }

        public async Task<bool> RolloverForm(FormRolloverInfoDto formRollover)
        {
            var parentForm = await _formRepository.GetFormByIdAsync(formRollover.FormId);
            if (parentForm == null)
            {
                return false;
            }
            var newFormToken = JObject.Parse(parentForm.Data);

            newFormToken[FormIoProps.Name] = formRollover.NewFormName;
            newFormToken[FormIoProps.ObjId] = null;

            var mwpMetaData = newFormToken[FormIoProps.MwpMetaData]?.Value<JObject>();
            if (mwpMetaData == null)
            {
                mwpMetaData = new JObject();
                newFormToken[FormIoProps.MwpMetaData] = mwpMetaData;
            }
            mwpMetaData[FormIoProps.Parent] = formRollover.FormId.ToString();

            if (!formRollover.IsForceRolloverAllComponents)
            {
                RemoveExcludedComponents(newFormToken);
            }

            await CreateCopiedDocumentForRolloverDocuments(newFormToken);

            var hierarchicalPath = mwpMetaData[FormIoProps.MwpProps.HierarchicalPath]?.Value<JArray>();
            if (hierarchicalPath == null)
            {
                hierarchicalPath = new JArray();
                mwpMetaData.Add(FormIoProps.MwpProps.HierarchicalPath, hierarchicalPath);
            }

            hierarchicalPath.Add(parentForm.Id);

            var parentNodes = mwpMetaData[FormIoProps.MwpProps.ParentNodes]?.Value<JArray>();
            if (parentNodes == null)
            {
                parentNodes = new JArray();
                mwpMetaData.Add(FormIoProps.MwpProps.ParentNodes, parentNodes);
            }
            parentNodes.Add(JToken.FromObject(new { id = parentForm.Id, version = parentForm.CurrentVersion }));

            var savedNewFormJson = await SaveForm(newFormToken);
            var savedNewFormToken = JObject.Parse(savedNewFormJson);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            await AttachFormIdToFilesInForm(savedNewFormToken);
            await _unitOfWorkManager.Current.SaveChangesAsync();
            if (formRollover.IsRolloverSubmission)
            {
                var savedNewFormId = new Guid(savedNewFormToken[FormIoProps.ObjId]!.Value<string>());
                await CopySubmissionsToNewForm(savedNewFormId, parentForm.Id, savedNewFormToken);
            }

            return true;
        }

        private async Task CreateCopiedDocumentForRolloverDocuments(JObject newFormToken)
        {
            var fileUploadTokens = ExtractAllFileUploadTokensInForm(newFormToken);
            foreach (var fileUploadToken in fileUploadTokens)
            {
                if (fileUploadToken["mwp"]?["documents"] is JArray documents && documents.Count > 0)
                {
                    if (fileUploadToken["mwp"]?["includeDocumentsInTheRollOver"]?.Value<bool>() != true)
                    {
                        documents.RemoveAll();
                        continue;
                    }
                    foreach (var fileToken in documents)
                    {
                        var originalFileId = fileToken.Value<string>("fileId");
                        var copiedResult = await _fileStorageClient.CopyFileById(originalFileId);
                        await _eventBus.PublishAsync(new UploadedFileEventData(copiedResult));
                        fileToken["fileId"] = copiedResult.FileId;
                    }

                }
            }
        }

        public async Task<JArray> GetChildrenFormByFormId(Guid formId)
        {
            return await _formRepository.GetChildrenFormByFormId(formId);
        }

        private void RemoveExcludedComponents(JToken newFormToken)
        {
            if (newFormToken?[FormIoProps.Components] == null)
            {
                return;
            }

            var components = newFormToken[FormIoProps.Components]!.Value<JArray>();

            var componentsAsList = components.Children().ToList();
            componentsAsList.ForEach(c =>
            {
                if (c[FormIoProps.Mwp]?[FormIoProps.IncludeInTheRollOver] == null ||
                    c[FormIoProps.Mwp]?[FormIoProps.IncludeInTheRollOver]!.Value<bool>() == false)
                {
                    c.Remove();
                }
                else
                {
                    if (c[FormIoProps.Components] != null)
                    {
                        RemoveExcludedComponents(c);
                    }
                }
            });
        }

        private async Task CopySubmissionsToNewForm(Guid newFormId, Guid parentFormId,
            JToken formToken)
        {
            var parentSubmissions = _submissionRepository.Where(x => x.FormId == parentFormId)
                .ToList();
            foreach (var parentSubmission in parentSubmissions)
            {
                var parentSubmissionJObj = JObject.Parse(parentSubmission.Data);
                var newSubmissionToken = JObject.Parse(parentSubmission.Data);
                newSubmissionToken.Remove(FormIoProps.ObjId);
                newSubmissionToken.Remove(FormIoProps.Form);
                newSubmissionToken.Remove(FormIoProps.ExternalIds);
                RemoveDataOfComponentThatNotIncludeInForm(formToken, newSubmissionToken);
                var metaJObj = new JObject
                {
                    { FormIoProps.CurrentVersion, GuidGenerator.Create() },
                    { FormIoProps.Parent, parentSubmissionJObj[FormIoProps.ObjId]?.Value<string>() }
                };
                var parentNodes = parentSubmissionJObj.Value<JObject>(FormIoProps.MwpMetaData)
                    ?.Value<JArray>(FormIoProps.MwpProps.ParentNodes) ?? new JArray();
                var parentNode = new JObject();
                parentNode.Add(FormIoProps.ParentNodeProps.Id, parentSubmissionJObj.Value<string>(FormIoProps.ObjId));
                parentNode.Add(FormIoProps.ParentNodeProps.Version, parentSubmissionJObj.Value<JObject>(FormIoProps.MwpMetaData)?.Value<string>(FormIoProps.CurrentVersion));
                parentNodes.Add(parentNode);
                metaJObj.Add(FormIoProps.MwpProps.ParentNodes, parentNodes);
                newSubmissionToken[FormIoProps.MwpMetaData] = metaJObj;
                await SaveSubmission(newFormId, newSubmissionToken);
            }

            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        private void RemoveDataOfComponentThatNotIncludeInForm(JToken formToken, JObject submissionToken)
        {
            if (formToken == null || submissionToken == null)
            {
                return;
            }

            var dataRoot = submissionToken[FormIoProps.Data]?.Value<JObject>();
            var components = formToken[FormIoProps.Components]?.Value<JArray>();

            RemovePropertiesThatNotInComponentList(dataRoot, components);
        }

        private void RemovePropertiesThatNotInComponentList(JObject data, JArray components)
        {
            if (data == null || components == null)
            {
                return;
            }

            var propKeys = data.Properties().Select(x => x.Name).ToList();
            var componentKeys = components.Children()
                .Select(x => x[FormIoProps.Key]?.Value<string>())
                .Where(x => x != null)
                .ToList();
            propKeys.ForEach(propKey =>
            {
                if (!componentKeys.Contains(propKey))
                {
                    data.Remove(propKey);
                }
            });
            foreach (var component in components)
            {
                var componentKey = component[FormIoProps.Key]?.Value<string>();
                var innerComponents = component[FormIoProps.Components]?.Value<JArray>();
                if (componentKey != null
                    && innerComponents != null
                    && data[componentKey]?.Value<JObject>() != null)
                {
                    RemovePropertiesThatNotInComponentList(
                        data[componentKey].Value<JObject>(),
                        innerComponents);
                }
            }
        }

        private void AppendMwpFormMetaData(Form form, JToken formToken)
        {
            var isNewForm = form.Data == null;
            if (isNewForm)
            {
                return;
            }

            var existFormToken = JObject.Parse(form.Data);
            if (existFormToken[FormIoProps.MwpMetaData] != null && formToken[FormIoProps.MwpMetaData] == null)
            {
                formToken[FormIoProps.MwpMetaData] = existFormToken[FormIoProps.MwpMetaData].DeepClone();
            }
            else
            {
                formToken[FormIoProps.MwpMetaData] = new JObject();
            }
        }

        private async Task<Form> GetFormForFormToken(JToken token)
        {
            var formId = token[FormIoProps.ObjId]?.Value<string>() == null
                ? Guid.Empty
                : new Guid(token[FormIoProps.ObjId].Value<string>());
            if (formId != Guid.Empty)
            {
                return await GetForm(formId);
            }
            return null;
        }

        private async Task<Form> GetForm(Guid formId)
        {
            return await _formRepository.GetFormByIdAsync(formId);
        }

        private Submission CreateNewSubmission(JToken token, Form form)
        {
            var newObjectId = GuidGenerator.Create();
            var jObj = JObject.Parse(token.ToString());
            jObj.Add(FormIoProps.ObjId, newObjectId);
            jObj.Add(FormIoProps.Form, form.Id);
            jObj.Add(FormIoProps.ExternalIds, new JArray());
            var submission = new Submission(newObjectId, form.Id)
            {
                CreationTime = Clock.Now,
                Data = jObj.ToString()
            };
            return submission;
        }

        private async Task<Tuple<Submission, Form>> GetExistSubmission(Guid id, Guid formId)
        {
            var form = await GetForm(formId);
            if (id == Guid.Empty)
            {
                return new Tuple<Submission, Form>(null, form);
            }

            var submission = await _submissionRepository.FindAsync(x => x.Id == id);
            //Handle case when parameter formId = null but submission already exist
            if (form == null && submission != null)
            {
                form = await GetForm(submission.FormId);
            }

            return new Tuple<Submission, Form>(submission, form);
        }

        private bool UpdateSubmission(Submission submission, JToken token)
        {
            var isModified = false;
            var oldDataToken = JObject.Parse(submission.Data);
            var isThereChangeInData = !JToken.DeepEquals(oldDataToken[FormIoProps.Data], token[FormIoProps.Data]);
            var isThereChangeInExtIds =
                !JToken.DeepEquals(oldDataToken[FormIoProps.ExternalIds], token[FormIoProps.ExternalIds]);
            if (isThereChangeInData || isThereChangeInExtIds)
            {
                submission.Data = token.ToString();
                isModified = true;
            }

            return isModified;
        }

        private async Task SaveSubmission(bool isNew, bool isModified, Submission submission)
        {
            if (!isNew && !isModified)
            {
                return;
            }

            if (isNew)
            {
                await _submissionRepository.InsertAsync(submission);
            }
            else
            {
                await _submissionRepository.UpdateAsync(submission);
            }
        }

        private async Task AppendExternalIdsToChildrenSubmissions(Submission submission, Form form)
        {
            if (submission == null || form == null || submission.Id == Guid.Empty)
            {
                return;
            }

            var formComponents = GetAllSubFormComponentsInForm(form);
            if (formComponents == null || formComponents.Length <= 0)
            {
                return;
            }

            var refSubFormKeys = formComponents
                .Where(x => x[FormIoProps.Reference] != null
                            && x[FormIoProps.Reference].Type == JTokenType.Boolean
                            && x[FormIoProps.Reference].Value<bool>())
                .Select(x => x[FormIoProps.Key]!.Value<string>())
                .ToArray();
            var subFormSubmissionIds = ExtractSubFormSubmissionIds(submission, refSubFormKeys);
            if (subFormSubmissionIds == null || subFormSubmissionIds.Length <= 0)
            {
                return;
            }

            var subFormSubmissions = GetSubmissionByObjectIds(subFormSubmissionIds);
            foreach (var subFormSubmission in subFormSubmissions)
            {
                var subFormSubmissionObj = JObject.Parse(subFormSubmission.Data);
                var subFormId = new Guid(subFormSubmissionObj[FormIoProps.Form]!.Value<string>());
                AppendSubFormParentExternalId(subFormSubmissionObj, submission.Id);
                await SaveSubmission(subFormId, subFormSubmissionObj);
            }
        }

        private void AppendSubFormParentExternalId(
            JObject subFormSubmissionObj,
            Guid submissionId)
        {
            JArray externalIds;
            if (subFormSubmissionObj[FormIoProps.ExternalIds] == null)
            {
                externalIds = new JArray();
                subFormSubmissionObj[FormIoProps.ExternalIds] = externalIds;
            }
            else
            {
                externalIds = subFormSubmissionObj[FormIoProps.ExternalIds].Value<JArray>();
            }

            var isRelationExist = externalIds.Any(x =>
                x[FormIoProps.Id] != null
                && new Guid(x[FormIoProps.Id].Value<string>()) == submissionId
                && x[FormIoProps.ObjType]?.Value<string>() == FormIoProps.Parent);
            if (!isRelationExist)
            {
                var extId = new JObject
                {
                    { FormIoProps.ObjId, GuidGenerator.Create().ToString() },
                    { FormIoProps.ObjType, FormIoProps.Parent },
                    { FormIoProps.Id, submissionId }
                };
                externalIds.Add(extId);
            }
        }

        private IList<Submission> GetSubmissionByObjectIds(Guid[] subFormSubmissionIds)
        {
            var submissions = _submissionRepository
                .Where(x => subFormSubmissionIds.Contains(x.Id))
                .ToList();
            return submissions;
        }

        protected virtual JToken[] GetAllSubFormComponentsInForm(Form form)
        {
            var formObj = JObject.Parse(form.Data);
            if (formObj[FormIoProps.Components] == null
                || formObj[FormIoProps.Components].Type != JTokenType.Array)
            {
                return null;
            }

            return formObj[FormIoProps.Components]
                .ToArray()
                .Where(x => x[FormIoProps.ObjType]?.Value<string>() == FormIoProps.Form)
                .ToArray();
        }

        private Guid[] ExtractSubFormSubmissionIds(
            Submission submission,
            string[] refSubFormKeys)
        {
            if (submission == null)
            {
                return null;
            }

            var submissionObj = JObject.Parse(submission.Data);
            if (submissionObj[FormIoProps.Data] == null
                || submissionObj[FormIoProps.Data].Type != JTokenType.Object
                || refSubFormKeys == null
                || refSubFormKeys.Length <= 0)
            {
                return null;
            }

            var results = new List<Guid>();
            var dataObj = submissionObj[FormIoProps.Data];
            foreach (var subFormKey in refSubFormKeys)
            {
                var subFormObj = dataObj[subFormKey];
                if (subFormObj != null
                    && subFormObj.Type == JTokenType.Object
                    && subFormObj[FormIoProps.ObjId] != null
                    && subFormObj[FormIoProps.ObjId].Type == JTokenType.String
                    && subFormObj[FormIoProps.Form] != null)
                {
                    results.Add(new Guid(subFormObj[FormIoProps.ObjId].Value<string>()));
                }
            }

            return results.ToArray();
        }


        private async Task ClearReferrerIdOfExistFiles(Guid submissionId)
        {
            var existingFileIds = await _fileStorageClient.GetFileIdsByReferredBy(submissionId.ToString());
            if (existingFileIds == null || existingFileIds.Length == 0)
            {
                return;
            }

            await _fileStorageClient.UpdateFilesReferredBy(existingFileIds, null, FileReferrerType.None);
        }

        private async Task AssignReferrerIdToFiles(Guid referrerId, string[] fileIds, FileReferrerType fileReferrerType)
        {
            if (fileIds == null || fileIds.Length == 0)
            {
                return;
            }

            await _fileStorageClient.UpdateFilesReferredBy(fileIds, referrerId, fileReferrerType);
            await _eventBus.PublishAsync(new AssignedReferrerIdToFileEventData(referrerId, fileIds, fileReferrerType));
        }

        public async Task UpdateFileInfoInSubmission(Guid submissionId, Guid currentFileId, UploadFileResult fileInfo)
        {
            var submission = await _submissionRepository.FindAsync(x => x.Id == submissionId);
            var form = await GetForm(submission.FormId);

            if (form == null || submission == null || string.IsNullOrEmpty(form.Data) || string.IsNullOrEmpty(submission.Data))
            {
                throw new EntityNotFoundException();
            }

            var formJObj = JObject.Parse(form.Data);
            var subJObj = JObject.Parse(submission.Data);

            var fileUploadTokens = ExtractAllFileUploadTokensInForm(formJObj);
            foreach (var fileUploadToken in fileUploadTokens)
            {
                var key = fileUploadToken.Value<string>(FormIoProps.Key);
                var fileToken = subJObj.SelectToken("$.data." + key + "[?(@.fileId=='" + currentFileId + "')]");
                if (fileToken != null)
                {
                    var jObj = new JObject
                    {
                        [FormIoProps.FileProps.FileId] = fileInfo.FileId,
                        [FormIoProps.FileProps.FileName] = fileInfo.FileName,
                        [FormIoProps.FileProps.FileSize] = fileInfo.FileSize,
                        [FormIoProps.FileProps.Sha1] = fileInfo.FileHash
                    };
                    fileToken.Replace(jObj);
                }
            }

            submission.Data = subJObj.ToString();
            await SaveSubmission(false, true, submission);
        }

        public async Task<ImportSubmissionResult> ImportSubmissionFromExcel(Guid formId, byte[] data)
        {
            var result = new ImportSubmissionResult();
            try
            {
                var form = await _formRepository.GetFormByIdAsync(formId);
                var formJObj = JObject.Parse(form.Data);

                var importer = new ExcelSubmissionImporter();
                var submissionJArray = importer.ImportSubmissions(formJObj, data);

                var submissions = submissionJArray.Children()
                    .Select(x => new Submission(GuidGenerator.Create(), formId)
                    {
                        Data = x.ToString()
                    }).ToList();

                result.ImportedSubmissionIds = submissions.Select(x => x.Id).ToArray();

                foreach (var submission in submissions)
                {
                    await _submissionRepository.InsertAsync(submission);
                }

                result.Success = true;
            }
            catch (Exception err)
            {
                result.Success = false;
                result.Message = err.Message;
            }

            return result;
        }

        public async Task<byte[]> ExportSubmissionToExcel(Guid formId, bool isBlank)
        {
            try
            {
                var form = await _formRepository.GetFormByIdAsync(formId);
                if (form == null)
                {
                    return null;
                }

                var formJObj = JObject.Parse(form.Data);
                var submissionJArray = new JArray();
                if (!isBlank)
                {
                    var submissions = _submissionRepository
                        .Where(x => x.FormId == formId)
                        .ToList();
                    submissions.ForEach(x => { submissionJArray.Add(JObject.Parse(x.Data)); });
                }

                var exporter = new ExcelSubmissionExporter();
                return exporter.Export(formJObj, submissionJArray);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task DeleteForm(Guid formId)
        {
            await _formRepository.DeleteAsync(new Form(formId));
        }

        public async Task DeleteSubmission(Guid formId, Guid submissionId)
        {
            await _submissionRepository.DeleteAsync(new Submission(submissionId, formId));
        }

        public async Task RestoreFormToVersion(Guid formId, Guid historyId)
        {
            var formJObj = await _formsStorageClient.GetFormHistory(formId, historyId.ToString());
            await SaveForm(formJObj);
        }
    }
}