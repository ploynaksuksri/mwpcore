using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Form.Dtos;
using Mwp.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mwp.Form
{
    public class FormAppServiceTest : MwpApplicationTestBase
    {
        public FormAppServiceTest()
        {
            _formAppService = GetRequiredService<IFormAppService>();
            _userConfigRepo = GetRequiredService<IRepository<UserFormConfig>>();
            _formRepo = GetRequiredService<IRepository<Form>>();
            _submissionRepo = GetRequiredService<IRepository<Submission>>();
        }

        private readonly IFormAppService _formAppService;
        private readonly IRepository<UserFormConfig> _userConfigRepo;
        private readonly IRepository<Form> _formRepo;
        private readonly IRepository<Submission> _submissionRepo;

        private async Task<Guid> CreateFormWithSubForm(Guid subFormId)
        {
            var mainFormJObj = new JObject();
            mainFormJObj["name"] = "TestMainForm";
            var components = new JArray();
            var text1Elemet = JObject.FromObject(
                new
                {
                    key = "txt1",
                    type = "textField",
                    label = "Text1"
                });
            components.Add(text1Elemet);
            var subFormElement = JObject.FromObject(
                new
                {
                    label = "Text1",
                    key = "sub1",
                    type = "form",
                    form = subFormId,
                    reference = true
                });
            components.Add(subFormElement);
            mainFormJObj["components"] = components;
            var saveMainFormJson = await _formAppService.SaveForm(mainFormJObj);
            var saveMainFormJObj = JObject.Parse(saveMainFormJson);
            return new Guid(saveMainFormJObj["_id"]!.Value<string>());
        }

        private async Task<string> CreateNewForm()
        {
            var token = new JObject();
            token["name"] = "TestForm";
            var components = new JArray();
            token["components"] = components;
            var json = await _formAppService.SaveForm(token);
            return json;
        }

        private async Task<string> CreateNewSubmission()
        {
            var saveFormJson = await CreateNewForm();
            var saveFormJObj = JObject.Parse(saveFormJson);
            var formId = new Guid(saveFormJObj["_id"]!.Value<string>());
            var obj = new
            {
                data = new
                {
                    firstName = "TESTName",
                    lastName = "TESTSurname",
                    birthDate = "1999-05-05",
                    submit = true
                }
            };
            var jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            var saveResult = await _formAppService.SaveSubmission(formId, jObj);
            return saveResult;
        }

        private async Task<JToken> InsertNewFormToDb()
        {
            var json = await CreateNewForm();
            var token = JObject.Parse(json);
            var newComponents = new JArray();
            var textField1JObj = new JObject
            {
                [FormIoProps.Key] = "text1",
                [FormIoProps.Label] = "TEXT1",
                [FormIoProps.ObjType] = "textfield",
                [FormIoProps.Mwp] = new JObject
                {
                    [FormIoProps.IncludeInTheRollOver] = true
                }
            };
            newComponents.Add(textField1JObj);
            var textField2JObj = new JObject
            {
                [FormIoProps.Key] = "text2",
                [FormIoProps.Label] = "TEXT2",
                [FormIoProps.ObjType] = "textfield"
            };
            newComponents.Add(textField2JObj);
            var textField3JObj = new JObject
            {
                [FormIoProps.Key] = "text3",
                [FormIoProps.Label] = "TEXT3",
                [FormIoProps.ObjType] = "textfield",
                [FormIoProps.Mwp] = new JObject
                {
                    [FormIoProps.IncludeInTheRollOver] = true
                }
            };
            newComponents.Add(textField3JObj);
            token[FormIoProps.Components] = newComponents;
            var savedFormJson = await _formAppService.SaveForm(token);
            return JObject.Parse(savedFormJson);
        }

        private async Task InsertSubmissionFromResourceFile(Guid formId,
            string embededResourceName)
        {
            var json =
                MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(typeof(FormAppServiceTest).Assembly, embededResourceName);
            var token = JObject.Parse(json);
            var savedJson = await _formAppService.SaveSubmission(formId, token);
            JObject.Parse(savedJson);
        }

        private async Task<JToken> InsertFormFromResourceFile(string formName, string embededResourceName)
        {
            var formJson =
                MwpEmbeddedResourceUtils.ReadStringFromEmbededResource(typeof(FormAppServiceTest).Assembly, embededResourceName);
            var token = JObject.Parse(formJson);
            token[FormIoProps.Name] = formName;
            var savedFormJson = await _formAppService.SaveForm(token);
            return JObject.Parse(savedFormJson);
        }

        [Fact]
        public async Task RolloverForm_WhenIsRolloverSubmissionSetToTrue_ShouldCopyAllParentFormSubmissionToNewForm()
        {
            var parentFormToken = await InsertNewFormToDb();

            var parentFormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>());
            var obj = new
            {
                data = new
                {
                    text1 = "Txt1",
                    submit = true
                }
            };
            var parentFormSubmission1JObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            var saveParentFormSubmission1ResultJson =
                await _formAppService.SaveSubmission(parentFormId, parentFormSubmission1JObj);
            var saveParentFormSubmission1ResultJObj = JObject.Parse(saveParentFormSubmission1ResultJson);
            saveParentFormSubmission1ResultJObj.ShouldNotBeNull();
            var parentFormSubmission1Id =
                new Guid(saveParentFormSubmission1ResultJObj[FormIoProps.ObjId]!.Value<string>());


            await WithUnitOfWorkAsync(async () =>
            {
                var existForm = await _formRepo.FindAsync(x => x.Id == parentFormId);
                existForm.ShouldNotBeNull();

                var existSubmission = await _submissionRepo.FindAsync(x => x.Id == parentFormSubmission1Id);
                existSubmission.ShouldNotBeNull();
            });

            var rollOverInfo = new FormRolloverInfoDto
            {
                NewFormName = "NewForm",
                FormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>()),
                IsRolloverSubmission = true
            };

            var result = await _formAppService.RolloverForm(rollOverInfo);
            result.ShouldBeTrue();

            await WithUnitOfWorkAsync(async () =>
            {
                var newForm = await _formRepo.FindAsync(x => x.ParentId == parentFormId);
                newForm.ShouldNotBeNull();

                var newFormSubmission = await _submissionRepo.FindAsync(x => x.FormId == newForm.Id);
                newFormSubmission.ShouldNotBeNull();
            });
        }


        [Fact]
        public async Task RolloverForm_WhenIsRolloverSubmissionSetToTrue_ShouldAssignParentSubmissionVersionInsideNewSubmission()
        {
            var parentFormToken = await InsertNewFormToDb();

            var parentFormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>());
            var obj = new
            {
                data = new
                {
                    text1 = "Txt1",
                    submit = true
                }
            };
            var parentFormSubmission1JObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            var saveParentFormSubmission1ResultJson =
                await _formAppService.SaveSubmission(parentFormId, parentFormSubmission1JObj);
            var saveParentFormSubmission1ResultJObj = JObject.Parse(saveParentFormSubmission1ResultJson);
            saveParentFormSubmission1ResultJObj.ShouldNotBeNull();
            var parentFormSubmission1Id =
                new Guid(saveParentFormSubmission1ResultJObj[FormIoProps.ObjId]!.Value<string>());


            await WithUnitOfWorkAsync(async () =>
            {
                var existForm = await _formRepo.FindAsync(x => x.Id == parentFormId);
                existForm.ShouldNotBeNull();

                var existSubmission = await _submissionRepo.FindAsync(x => x.Id == parentFormSubmission1Id);
                existSubmission.ShouldNotBeNull();
            });

            var rollOverInfo = new FormRolloverInfoDto
            {
                NewFormName = "NewForm",
                FormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>()),
                IsRolloverSubmission = true
            };

            var result = await _formAppService.RolloverForm(rollOverInfo);
            result.ShouldBeTrue();

            await WithUnitOfWorkAsync(async () =>
            {
                var newForm = await _formRepo.FindAsync(x => x.ParentId == parentFormId);
                newForm.ShouldNotBeNull();

                var newFormSubmission = await _submissionRepo.FindAsync(x => x.FormId == newForm.Id);
                newFormSubmission.ShouldNotBeNull();

                var submissionJObj = JObject.Parse(newFormSubmission.Data);
                var mwpMetaData = submissionJObj.Value<JObject>(FormIoProps.MwpMetaData);
                mwpMetaData.ShouldNotBeNull();
                mwpMetaData[FormIoProps.Parent].ShouldNotBeNull();
                mwpMetaData[FormIoProps.Parent].Value<string>().ShouldBe(parentFormSubmission1Id.ToString());
                var parentNodes = mwpMetaData.Value<JArray>(FormIoProps.MwpProps.ParentNodes);
                parentNodes.ShouldNotBeNull();
                parentNodes.Count.ShouldBe(1);
                var parentNode = parentNodes[0];
                parentNode.ShouldNotBeNull();

            });
        }


        [Fact]
        public async Task
            RolloverForm_WhenOnlySomeComponentInFormIncludeInTheRollOver_ShouldCopyOnlyThoseComponentsToNewForm()
        {
            var parentFormToken = await InsertFormFromResourceFile(
                "ParentForm",
                "Mwp.Form.json.nested-form-for-rollover-test.json");

            var parentFormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>());
            await InsertSubmissionFromResourceFile(
                parentFormId,
                "Mwp.Form.json.submission-of-nested-form-for-rollover-test.json");
            var rollOverInfo = new FormRolloverInfoDto
            {
                NewFormName = "NewForm",
                FormId = parentFormId,
                IsRolloverSubmission = true
            };

            await _formAppService.RolloverForm(rollOverInfo);

            await WithUnitOfWorkAsync(async () =>
            {
                var newForm = await _formRepo.Where(x => x.ParentId == parentFormId).FirstOrDefaultAsync();
                newForm.ShouldNotBeNull();
                var newFormToken = JObject.Parse(newForm.Data);
                newFormToken.SelectTokens("$..key").Count().ShouldBe(3);
                var newFormId = new Guid(newFormToken[FormIoProps.ObjId]!.Value<string>());
                var newSubmission = await _submissionRepo.Where(x => x.FormId == newFormId).FirstOrDefaultAsync();
                newSubmission.ShouldNotBeNull();
                var newSubmissionToken = JObject.Parse(newSubmission.Data);
                newSubmissionToken[FormIoProps.Data]!["container"]!["textField1"].ShouldBeNull();
            });
        }

        [Fact]
        public async Task GetParentFormsByFormId()
        {
            var parentFormToken = await InsertNewFormToDb();
            var rollOverInfo = new FormRolloverInfoDto
            {
                NewFormName = "NewForm",
                FormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>())
            };
            await _formAppService.RolloverForm(rollOverInfo);

            Form savedNewForm = null;
            Form parentForm = null;
            var parentFormVersion = Guid.Empty;
            await WithUnitOfWorkAsync(async () =>
            {
                parentForm = await _formRepo.GetAsync(x => x.Id == rollOverInfo.FormId);
                savedNewForm = await _formRepo.Where(x => x.Name == rollOverInfo.NewFormName).FirstOrDefaultAsync();
                parentFormVersion = parentForm.CurrentVersion;
            });
            parentForm.ShouldNotBeNull();
            savedNewForm.ShouldNotBeNull();

            var rollover2 = new FormRolloverInfoDto
            {
                NewFormName = "NewForm2",
                FormId = savedNewForm.Id
            };

            await _formAppService.RolloverForm(rollover2);
            Form savedNewForm2 = null;
            await WithUnitOfWorkAsync(async () =>
            {
                savedNewForm2 = await _formRepo.Where(x => x.Name == rollover2.NewFormName).FirstOrDefaultAsync();
            });
            savedNewForm2.ShouldNotBeNull();

            var parentForms = await _formAppService.GetParentFormsByFormId(savedNewForm2.Id);
            parentForms.ShouldNotBeNull();
            parentForms.Count.ShouldBe(2);

            var parentForm1JObj = parentForms.FirstOrDefault(x=>x[FormIoProps.ObjId]?.Value<string>() == parentForm.Id.ToString());
            parentForm1JObj.ShouldNotBeNull();
            parentForm1JObj[FormIoProps.CurrentVersion]?.Value<Guid>().ShouldNotBeNull();
            parentForm1JObj[FormIoProps.CurrentVersion]?.Value<Guid>().ShouldBe(parentFormVersion);

            var parentForm2JObj =  parentForms.FirstOrDefault(x=>x[FormIoProps.ObjId]?.Value<string>() == savedNewForm.Id.ToString());
            parentForm2JObj.ShouldNotBeNull();
            parentForm2JObj[FormIoProps.CurrentVersion]?.Value<Guid>().ShouldNotBeNull();
            parentForm2JObj[FormIoProps.CurrentVersion]?.Value<Guid>().ShouldBe(savedNewForm.CurrentVersion);
        }

        [Fact]
        public async Task RolloverForm_WhenParentFormExist_ShouldCreateNewForm()
        {
            var parentFormToken = await InsertNewFormToDb();
            long existFormCountBeforeRollover = 0;
            await WithUnitOfWorkAsync(async () => { existFormCountBeforeRollover = await _formRepo.GetCountAsync(); });

            var rollOverInfo = new FormRolloverInfoDto
            {
                NewFormName = "NewForm",
                FormId = new Guid(parentFormToken[FormIoProps.ObjId]!.Value<string>())
            };

            var result = await _formAppService.RolloverForm(rollOverInfo);
            result.ShouldBeTrue();

            Form savedNewForm = null;
            Form parentForm = null;
            await WithUnitOfWorkAsync(async () =>
            {
                parentForm = await _formRepo.GetAsync(x => x.Id == rollOverInfo.FormId);
                var existFormCount = await _formRepo.GetCountAsync();
                existFormCount.ShouldBe(existFormCountBeforeRollover + 1);
                savedNewForm = await _formRepo.Where(x => x.Name == rollOverInfo.NewFormName).FirstOrDefaultAsync();
            });
            parentForm.ShouldNotBeNull();
            savedNewForm.ShouldNotBeNull();
            var parentFormId = parentFormToken[FormIoProps.ObjId]?.Value<string>();
            savedNewForm.HierarchicalPath.ShouldBe(parentFormId);
            savedNewForm.ParentId.ShouldNotBeNull();
            savedNewForm.ParentId.ShouldBe(new Guid(parentFormId));

            var savedNewFormToken = JObject.Parse(savedNewForm.Data);
            savedNewFormToken[FormIoProps.MwpMetaData].ShouldNotBeNull();
            savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.MwpProps.HierarchicalPath].ShouldNotBeNull();
            savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.MwpProps.ParentNodes].ShouldNotBeNull();
            savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.Parent].ShouldNotBeNull();
            savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.Parent].Value<string>().ShouldBe(parentFormId);
            var parentNodes = savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.MwpProps.ParentNodes].Value<JArray>();
            parentNodes.Count.ShouldBe(1);
            var parentNode1 = parentNodes[0];
            parentNode1.ShouldNotBeNull();
            parentNode1.Value<string>(FormIoProps.ParentNodeProps.Id).ShouldBe(parentForm.Id.ToString());
            parentNode1.Value<string>(FormIoProps.ParentNodeProps.Version).ShouldBe(parentForm.CurrentVersion.ToString());
            var newFormParanetIds = savedNewFormToken[FormIoProps.MwpMetaData][FormIoProps.MwpProps.HierarchicalPath]
                .Value<JArray>();
            newFormParanetIds.ShouldNotBeNull();
            newFormParanetIds.Count.ShouldBe(1);

            var rollover2 = new FormRolloverInfoDto
            {
                NewFormName = "NewForm2",
                FormId = savedNewForm.Id
            };

            var result2 = await _formAppService.RolloverForm(rollover2);
            result2.ShouldBeTrue();

            Form savedNewForm2 = null;
            await WithUnitOfWorkAsync(async () =>
            {
                var existFormCount = await _formRepo.GetCountAsync();
                existFormCount.ShouldBe(existFormCountBeforeRollover + 2);
                savedNewForm2 = await _formRepo.Where(x => x.Name == rollover2.NewFormName).FirstOrDefaultAsync();
            });
            savedNewForm2.ShouldNotBeNull();
            savedNewForm2.HierarchicalPath.ShouldBe(parentFormId + "/" + savedNewForm.Id);
            savedNewForm2.ParentId.ShouldNotBeNull();
            savedNewForm2.ParentId.ShouldBe(savedNewForm.Id);

            var savedNewForm2Token = JObject.Parse(savedNewForm2.Data);
            savedNewForm2Token[FormIoProps.MwpMetaData].ShouldNotBeNull();
            savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.MwpProps.HierarchicalPath].ShouldNotBeNull();
            savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.MwpProps.ParentNodes].ShouldNotBeNull();
            savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.Parent].ShouldNotBeNull();
            savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.Parent].Value<string>().ShouldBe(savedNewForm.Id.ToString());
            parentNodes = savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.MwpProps.ParentNodes].Value<JArray>();
            parentNodes.Count.ShouldBe(2);
            parentNode1 = parentNodes[0];
            parentNode1.Value<string>(FormIoProps.ParentNodeProps.Id).ShouldBe(parentForm.Id.ToString());
            parentNode1.Value<string>(FormIoProps.ParentNodeProps.Version).ShouldBe(parentForm.CurrentVersion.ToString());

            var parentNode2 = parentNodes[1];
            parentNode2.Value<string>(FormIoProps.ParentNodeProps.Id).ShouldBe(savedNewForm.Id.ToString());
            parentNode2.Value<string>(FormIoProps.ParentNodeProps.Version).ShouldBe(savedNewForm.CurrentVersion.ToString());

            var newForm2ParanetIds = savedNewForm2Token[FormIoProps.MwpMetaData][FormIoProps.MwpProps.HierarchicalPath]
                .Value<JArray>();
            newForm2ParanetIds.ShouldNotBeNull();
            newForm2ParanetIds.Count.ShouldBe(2);
        }


        [Fact]
        public async Task RolloverForm_WhenParentFormNotExist_ShouldReturnFalse()
        {
            var rollOverInfo = new FormRolloverInfoDto
            {
                FormId = Guid.NewGuid()
            };

            var result = await _formAppService.RolloverForm(rollOverInfo);
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task SaveForm_WhenSaveNewForm_ShouldGenerateNewFormId()
        {
            var json = await CreateNewForm();

            json.ShouldNotBeNullOrEmpty();
            var resultToken = JObject.Parse(json);
            resultToken.ShouldNotBeNull();
            resultToken["_id"]!.Value<string>().ShouldNotBeNull();
            var formId = new Guid(resultToken["_id"].Value<string>());
            formId.ShouldNotBe(Guid.Empty);

            var existFormJson = await _formAppService.GetFormByIdAsJsonAsync(formId);
            existFormJson.ShouldNotBeNullOrEmpty();
            var existFormJObj = JObject.Parse(existFormJson);
            existFormJObj.ShouldNotBeNull();
            new Guid(existFormJObj["_id"]!.Value<string>()).ShouldBe(formId);
            existFormJObj["name"]!.Value<string>().ShouldBe("TestForm");

            await WithUnitOfWorkAsync(async () =>
            {
                var savedForm = await _formRepo.GetAsync(x => x.Id == formId);
                savedForm.ShouldNotBeNull();
                savedForm.CurrentVersion.ShouldNotBe(Guid.Empty);
                var formHistory = _formAppService.GetFormHistory(formId, savedForm.CurrentVersion.ToString());
                formHistory.ShouldNotBeNull();
            });
        }

        [Fact]
        public async Task SaveForm_WhenUpdateExistForm_ShouldUpdateSuccessfully()
        {
            var json = await CreateNewForm();
            var token = JObject.Parse(json);
            var newComponents = new JArray();
            var textFieldJObj = new JObject
            {
                ["key"] = "text1",
                ["label"] = "TEXT1",
                ["type"] = "textfield"
            };
            newComponents.Add(textFieldJObj);

            token["components"] = newComponents;
            await _formAppService.SaveForm(token);

            var formId = new Guid(token["_id"]!.Value<string>());
            var getFormJson = await _formAppService.GetFormByIdAsJsonAsync(formId);
            var getFormJObj = JObject.Parse(getFormJson);

            new Guid(getFormJObj["_id"]!.Value<string>()).ShouldBe(formId);
            getFormJObj["components"]?.Type.ShouldBe(JTokenType.Array);
            var getFormComponents = getFormJObj["components"]!.Value<JArray>();
            getFormComponents.Count.ShouldBe(1);
            getFormComponents[0]["key"]!.Value<string>().ShouldBe("text1");
        }

        [Fact]
        public async Task SaveForm_WhenSaveFormContainMergeData_ShouldUpdateParentNodesInMwpMetaData()
        {
            var json = await CreateNewForm();
            var token = JObject.Parse(json);
            var newComponents = new JArray();
            var textFieldJObj = new JObject
            {
                ["key"] = "text1",
                ["label"] = "TEXT1",
                ["type"] = "textfield"
            };
            newComponents.Add(textFieldJObj);

            token["components"] = newComponents;
            var parentFormId = Guid.NewGuid().ToString();
            var parentFormVersion1 = Guid.NewGuid().ToString();
            token.Add(FormIoProps.MergeData, JObject.FromObject(new { parentFormId, parentFormVersion = parentFormVersion1 }));
            await _formAppService.SaveForm(token);

            var formId = new Guid(token["_id"]!.Value<string>());
            var getFormJson = await _formAppService.GetFormByIdAsJsonAsync(formId);
            var getFormJObj = JObject.Parse(getFormJson);
            getFormJObj[FormIoProps.MergeData].ShouldBeNull();

            var metaDataJObj = getFormJObj[FormIoProps.MwpMetaData]?.Value<JObject>();
            metaDataJObj.ShouldNotBeNull();

            var parentNodes = metaDataJObj[FormIoProps.MwpProps.ParentNodes]?.Value<JArray>();
            parentNodes.ShouldNotBeNull();
            parentNodes.Count.ShouldBe(1);

            var parentNode = parentNodes[0];
            parentNode[FormIoProps.ParentNodeProps.Id].ShouldBe(parentFormId);
            parentNode[FormIoProps.ParentNodeProps.Version].ShouldBe(parentFormVersion1);
        }

        [Fact]
        public async Task SaveSubmission_WhenFormContainSubForm_ShouldSaveSuccessfully()
        {
            var insertSubmissionJson = await CreateNewSubmission();
            var insertSubmissionJObj = JObject.Parse(insertSubmissionJson);
            var subFormId = new Guid(insertSubmissionJObj["form"]!.Value<string>());
            var formId = await CreateFormWithSubForm(subFormId);
            var obj = new
            {
                data = new
                {
                    firstName = "SubName1",
                    lastName = "SubSurname1",
                    birthDate = "1966-06-06",
                    submit = true
                }
            };
            var jObj = JObject.FromObject(obj);
            var subFormSubmissionJson = await _formAppService.SaveSubmission(subFormId, jObj);
            var subFormSubmissionJObj = JObject.Parse(subFormSubmissionJson);
            var subFormSubmissionId = new Guid(subFormSubmissionJObj["_id"]!.Value<string>());
            var mainFormSubMissionJObj = JObject.FromObject(
                new
                {
                    data = new
                    {
                        sub1 = new
                        {
                            _id = subFormSubmissionId,
                            form = subFormId
                        },
                        txt1 = "TEST"
                    }
                });
            var insertMainFormJson = await _formAppService.SaveSubmission(formId, mainFormSubMissionJObj);
            insertMainFormJson.ShouldNotBeNullOrEmpty();
            var insertMainFormJObj = JObject.Parse(insertMainFormJson);
            var insertedMainFormSubmissionId = new Guid(insertMainFormJObj["_id"]!.Value<string>());

            var getMainFormSubmissionJson = await _formAppService.GetSubmissionByIdAsJsonAsync(
                formId,
                insertedMainFormSubmissionId);
            getMainFormSubmissionJson.ShouldNotBeNullOrEmpty();

            var updatedSubFormSubmissionJson = await _formAppService.GetSubmissionByIdAsJsonAsync(
                subFormId,
                subFormSubmissionId);
            updatedSubFormSubmissionJson.ShouldNotBeNullOrEmpty();
            var updatedSubFormSubmissionJObj = JObject.Parse(updatedSubFormSubmissionJson);
            updatedSubFormSubmissionJObj["externalIds"].ShouldNotBeNull();
            var externalIds = updatedSubFormSubmissionJObj["externalIds"].Value<JArray>();
            externalIds.Count.ShouldBe(1);
            new Guid(externalIds[0]["id"]!.Value<string>()).ShouldBe(insertedMainFormSubmissionId);
        }

        [Fact]
        public async Task SaveSubmission_WhenSaveNewSubmission_ShouldGenerateNewSubmissionId()
        {
            var saveResult = await CreateNewSubmission();
            saveResult.ShouldNotBeNullOrEmpty();
            var resultToken = JObject.Parse(saveResult);
            resultToken.ShouldNotBeNull();
            resultToken["_id"]!.Value<string>().ShouldNotBeNull();
            var submissionId = new Guid(resultToken["_id"].Value<string>());
            submissionId.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task SaveSubmission_WhenSaveSubmission_ShouldGenerateNewSubmissionVersion()
        {
            var saveResult = await CreateNewSubmission();
            saveResult.ShouldNotBeNullOrEmpty();
            var resultToken = JObject.Parse(saveResult);
            resultToken.ShouldNotBeNull();
            resultToken["_id"]!.Value<string>().ShouldNotBeNull();
            var submissionId = new Guid(resultToken["_id"].Value<string>());

            var metaData = resultToken.Value<JObject>(FormIoProps.MwpMetaData);
            metaData.ShouldNotBeNull();
            var version1 = metaData.Value<string>(FormIoProps.CurrentVersion);
            version1.ShouldNotBeNull();

            var submissionHistory = await _formAppService.GetSubmissionHistory(submissionId, version1);
            submissionHistory.ShouldNotBeNull();

            var formId = new Guid(resultToken.Value<string>(FormIoProps.Form));
            resultToken[FormIoProps.Data]!["firstName"] = "TEST_NAME2";
            saveResult = await _formAppService.SaveSubmission(formId, resultToken);

            resultToken = JObject.Parse(saveResult);
            metaData = resultToken.Value<JObject>(FormIoProps.MwpMetaData);
            metaData.ShouldNotBeNull();

            var version2 = metaData.Value<string>(FormIoProps.CurrentVersion);
            version2.ShouldNotBe(version1);

            submissionHistory = await _formAppService.GetSubmissionHistory(submissionId, version2);
            submissionHistory.ShouldNotBeNull();
        }

        [Fact]
        public async Task SaveSubmission_WhenSubmissionExistButInputFormIdIsGuidEmpty_ShouldSaveSuccessfully()
        {
            var insertSubmissionJson = await CreateNewSubmission();
            var insertSubmissionJObj = JObject.Parse(insertSubmissionJson);
            var newData = new
            {
                firstName = "Name2",
                lastName = "TESTSurname",
                birthDate = "1999-05-05",
                submit = true
            };

            insertSubmissionJObj["data"] = JObject.FromObject(newData);
            var updateSubmissionJson = await _formAppService.SaveSubmission(Guid.Empty, insertSubmissionJObj);

            var formId = new Guid(insertSubmissionJObj["form"]!.Value<string>());
            updateSubmissionJson.ShouldNotBeNullOrEmpty();

            var submissionId = new Guid(insertSubmissionJObj["_id"]!.Value<string>());
            var getSubmissionJson = await _formAppService.GetSubmissionByIdAsJsonAsync(formId, submissionId);
            getSubmissionJson.ShouldNotBeNullOrEmpty();
            var getSubmissionJObj = JObject.Parse(getSubmissionJson);
            getSubmissionJObj.SelectToken("$.data.firstName")!.Value<string>().ShouldBe("Name2");
        }

        [Fact]
        public async Task SaveSubmission_WhenSubmissionFormIdIsInvalid_ShouldThrowException()
        {
            var obj = new
            {
                data = new
                {
                    firstName = "TESTName",
                    lastName = "TESTSurname",
                    birthDate = "1999-05-05",
                    submit = true
                }
            };
            var jObj = JObject.FromObject(obj);

            var exception = await Assert.ThrowsAsync<Exception>(async () => { await _formAppService.SaveSubmission(Guid.NewGuid(), jObj); });
            exception.Message.ShouldBe("Form not found.");
        }

        [Fact]
        public async Task SaveSubmission_WhenUpdateExistSubmission_ShouldUpdateSuccessfully()
        {
            var insertSubmissionJson = await CreateNewSubmission();
            var insertSubmissionJObj = JObject.Parse(insertSubmissionJson);
            var newData = new
            {
                firstName = "Name2",
                lastName = "TESTSurname",
                birthDate = "1999-05-05",
                submit = true
            };

            insertSubmissionJObj["data"] = JObject.FromObject(newData);
            var formId = new Guid(insertSubmissionJObj["form"]!.Value<string>());
            var updateSubmissionJson = await _formAppService.SaveSubmission(formId, insertSubmissionJObj);
            updateSubmissionJson.ShouldNotBeNullOrEmpty();

            var submissionId = new Guid(insertSubmissionJObj["_id"]!.Value<string>());
            var getSubmissionJson = await _formAppService.GetSubmissionByIdAsJsonAsync(formId, submissionId);
            getSubmissionJson.ShouldNotBeNullOrEmpty();
            var getSubmissionJObj = JObject.Parse(getSubmissionJson);
            getSubmissionJObj.SelectToken("$.data.firstName")!.Value<string>().ShouldBe("Name2");
        }

        [Fact]
        public async Task SaveUserFormConfig_WhenDataIsValid_ShouldSaveSuccessfully()
        {
            var userId = Guid.NewGuid();

            await WithUnitOfWorkAsync(async () =>
            {
                var existUsrCfg = await _userConfigRepo.FindAsync(x => x.Id == userId);
                existUsrCfg.ShouldBeNull();
            });
            var usrFormBuilderCfg = new
            {
                builder = new
                {
                    customBasic = false
                }
            };
            var token = JObject.FromObject(usrFormBuilderCfg);
            await _formAppService.SaveFormBuilderConfiguration(userId, token);
            await WithUnitOfWorkAsync(async () =>
            {
                var savedUsrCfg = await _userConfigRepo.FindAsync(x => x.Id == userId);
                savedUsrCfg.ShouldNotBeNull();
                savedUsrCfg.FormBuilderConfig.ShouldNotBeNullOrEmpty();
                var savedToken = JObject.Parse(savedUsrCfg.FormBuilderConfig);
                savedToken["userId"]!.Value<string>().ShouldNotBeNullOrEmpty();
                savedToken["builder"].ShouldNotBeNull();
                savedToken["builder"]["customBasic"]!.Value<bool>().ShouldBeFalse();
            });

            var usrCfgToken = await _formAppService.GetFormBuilderConfiguration(userId);
            usrCfgToken.ShouldNotBeNull();
            usrCfgToken["userId"]!.Value<string>().ShouldNotBeNullOrEmpty();
            usrCfgToken["builder"].ShouldNotBeNull();
            usrCfgToken["builder"]["customBasic"]!.Value<bool>().ShouldBeFalse();
        }
    }
}