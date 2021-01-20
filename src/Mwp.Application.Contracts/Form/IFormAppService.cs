using System;
using System.Threading.Tasks;
using Mwp.File;
using Mwp.Form.Dtos;
using Newtonsoft.Json.Linq;
using Volo.Abp.Application.Services;

namespace Mwp.Form
{
    public interface IFormAppService : IApplicationService
    {
        Task<string> GetFormByIdAsJsonAsync(Guid formId);
        Task<string> GetSubmissionByIdAsJsonAsync(Guid formId, Guid submissionId);
        Task<string> SaveSubmission(Guid formId, JToken token);
        Task<string> SaveForm(JToken token);
        Task<string> ListForm(string searchText, long skip, long take);
        Task<string> ListSubmission(Guid formId, long skip, long take);
        Task<JArray> GetFormsForLookUp(long limit, string[] columns);
        Task<JToken> SaveFormBuilderConfiguration(Guid userId, JToken formBuilderConfig);
        Task<JToken> GetFormBuilderConfiguration(Guid userId);
        Task<JObject> GetFormHistory(Guid formId, string rowKey);
        Task<JObject> GetSubmissionHistory(Guid submissionId, string rowKey);
        Task<JArray> ListFormHistory(Guid formId);
        Task<JArray> ListSubmissionHistory(Guid submissionId);
        Task<bool> RolloverForm(FormRolloverInfoDto formRollover);
        Task<JArray> GetChildrenFormByFormId(Guid formId);
        Task UpdateFileInfoInSubmission(Guid submissionId, Guid currentFileId, UploadFileResult fileInfo);
        Task<ImportSubmissionResult> ImportSubmissionFromExcel(Guid formId, byte[] data);
        Task<byte[]> ExportSubmissionToExcel(Guid formId, bool isBlank);
        Task DeleteForm(Guid formId);
        Task DeleteSubmission(Guid formId, Guid submissionId);
        Task RestoreFormToVersion(Guid formId, Guid historyId);

        string[] GetAllFileIdsInForm(JObject formJObj);
        Task<JArray> GetParentFormsByFormId(Guid formId);
    }
}