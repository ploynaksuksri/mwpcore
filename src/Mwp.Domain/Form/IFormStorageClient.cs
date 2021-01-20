using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Mwp.Form
{
    public interface IFormStorageClient
    {
        Task<string> SaveFormHistory(Form form);

        Task<JObject> GetFormHistory(Guid formId, string rowKey);

        Task<string> SaveSubmissionHistory(Submission submission);

        Task<JObject> GetSubmissionHistory(Guid submissionId, string rowKey);

        Task<JArray> ListFormHistory(Guid formId);

        Task<JArray> ListSubmissionHistory(Guid submissionId);

        Task<string> SaveMessageToFormBlobContainer(string message);

        Task<string> GetMessageFromFormBlobContainer(string hash);
    }
}