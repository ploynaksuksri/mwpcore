using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Volo.Abp.Domain.Repositories;

namespace Mwp.Form.Repository
{
    public interface IFormRepository : IRepository<Form>
    {
        Task<JToken> ListSubmission(Guid? tenantId, Guid formId, long skip, long take);

        Task<JToken> ListForm(Guid? tenantId, string searchText, long skip, long take);

        Task<JArray> GetFormsForLookUp(Guid? tenantId, long limit, string[] columns);

        Task<Form> GetFormByIdAsync(Guid id);
        Task<JArray> GetChildrenFormByFormId(Guid formId);
        Task<Form> GetFormByNameAsync(string formName);

        Task<IList<Submission>> GetSubmissionByIds(Guid[] ids);
    }
}