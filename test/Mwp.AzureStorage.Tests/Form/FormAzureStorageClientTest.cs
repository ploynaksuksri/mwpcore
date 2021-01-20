using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mwp.Form;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Mwp.AzureStorage.Form
{
    public class FormAzureStorageClientTest : MwpAzureStorageTestBase
    {
        private readonly IFormStorageClient _client;
        private readonly ITestOutputHelper _output;

        public FormAzureStorageClientTest(ITestOutputHelper output)
        {
            _output = output;
            _client = GetRequiredService<IFormStorageClient>();
        }

        [Fact]
        public async Task SaveFormHistory_WhenCalled_ShouldSaveDataSuccessfully()
        {
            var form = CreateForm();

            var rowKey = await _client.SaveFormHistory(form);

            rowKey.ShouldNotBeNullOrEmpty();

            var formHistory = await _client.GetFormHistory(form.Id, rowKey);
            formHistory.ShouldNotBeNull();
            formHistory["_id"]!.Value<string>().ShouldBe((string)form.Id.ToString());
            formHistory["title"]!.Value<string>().ShouldBe("TestForm");
        }

        [Fact]
        public async Task SaveSubmissionHistory_WhenCalled_ShouldSaveDataSuccessfully()
        {
            var submission = CreateSubmission();

            var rowKey = await _client.SaveSubmissionHistory(submission);

            rowKey.ShouldNotBeNullOrEmpty();

            var submissionHistory = await _client.GetSubmissionHistory(submission.Id, rowKey);
            new Guid(submissionHistory["_id"]!.Value<string>()).ShouldBe(submission.Id);
            submissionHistory["data"]?["testData"]!.Value<string>().ShouldBe("TEST DATA");
        }

        [Fact]
        public async Task ListFormHistory_WhenCalled_ShouldReturnDataSuccessfully()
        {
            var form = CreateForm();

            var formHistories = await _client.ListFormHistory(form.Id);

            formHistories.ShouldNotBeNull();
            formHistories.Count.ShouldBe(0);

            await _client.SaveFormHistory(form);

            formHistories = await _client.ListFormHistory(form.Id);

            formHistories.ShouldNotBeNull();
            formHistories.Count.ShouldBe(1);
        }

        [Fact]
        public async Task ListSubmissionHistory_WhenCalled_ShouldReturnDataSuccessfully()
        {
            var submission = CreateSubmission();

            var submissionHistories = await _client.ListSubmissionHistory(submission.Id);

            submissionHistories.ShouldNotBeNull();
            submissionHistories.Count.ShouldBe(0);

            await _client.SaveSubmissionHistory(submission);

            submissionHistories = await _client.ListSubmissionHistory(submission.Id);

            submissionHistories.ShouldNotBeNull();
            submissionHistories.Count.ShouldBe(1);
        }

        [Fact]
        public async Task SaveAndGetMessageFromBlobContainer()
        {
            var message = "TEST1234TEST";
            var hash = await _client.SaveMessageToFormBlobContainer(message);
            var result = await _client.GetMessageFromFormBlobContainer(hash);
            result.ShouldBe(message);
        }

        private Mwp.Form.Form CreateForm()
        {
            var formId = Guid.NewGuid();
            var obj = new
            {
                _id = formId,
                title = "TestForm",
                components = new List<object>
                {
                    new
                    {
                        type = "textField",
                        key = "text1",
                        label = "Text1"
                    }
                }
            };
            var formToken = JObject.FromObject(obj);
            var form = new Mwp.Form.Form(formId)
            {
                Data = formToken.ToString(),
                Name = "TestForm"
            };
            return form;
        }

        private Submission CreateSubmission()
        {
            var submissionId = Guid.NewGuid();
            var formId = Guid.NewGuid();
            var submission = new Submission(submissionId, formId);
            submission.Data = JObject.FromObject(
                new
                {
                    _id = submissionId,
                    data = new
                    {
                        testData = "TEST DATA"
                    },
                    mwp_metadata = new
                    {
                        currentVersion = Guid.NewGuid().ToString()
                    }
                }).ToString();
            return submission;
        }
    }
}