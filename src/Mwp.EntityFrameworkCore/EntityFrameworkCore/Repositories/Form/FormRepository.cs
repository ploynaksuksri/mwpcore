using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mwp.Form;
using Mwp.Form.Repository;
using Newtonsoft.Json.Linq;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Mwp.EntityFrameworkCore.Repositories.Form
{
    public class FormRepository : EfCoreRepository<MwpDbContext, Mwp.Form.Form, Guid>, IFormRepository
    {
        public FormRepository(IDbContextProvider<MwpDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<JArray> GetFormsForLookUp(Guid? tenantId, long limit, string[] columns)
        {
            EnsureConnectionOpen();
            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @jsonResult NVARCHAR(MAX) = (");
            sb.AppendLine("select top(@limit)");
            var colStrs = new List<string>();
            foreach (var col in columns)
            {
                colStrs.Add($"JSON_VALUE([Data],'$.{col}') AS \"{col}\"");
            }

            sb.AppendLine(string.Join(",\r\n", colStrs));
            sb.AppendLine("from mwp.Forms with(nolock)");
            sb.AppendLine("where [IsDeleted] = 0");
            sb.AppendLine(tenantId != null ? "and TenantId = @tenantId" : "and TenantId is null");
            sb.AppendLine("order by \"title\"");
            sb.AppendLine("for json path");
            sb.AppendLine(");");
            sb.AppendLine("SELECT @jsonResult;");
            var query = sb.ToString();

            var parameters = new Dictionary<string, object> { { "@limit", limit } };
            if (tenantId.HasValue)
            {
                parameters.Add("@tenantId", tenantId.Value);
            }

            using (var command = DbContext.Database.CreateQueryCommand(query, parameters))
            {
                var result = await command.ExecuteScalarAsync();
                return JArray.Parse((result as string)!);
            }
        }


        public async Task<Mwp.Form.Form> GetFormByIdAsync(Guid id)
        {
            return await DbContext.Forms.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<JArray> GetChildrenFormByFormId(Guid formId)
        {
            var form = await DbContext.Forms.FindAsync(formId);
            if (form == null)
            {
                return new JArray();
            }

            var parentPath = form.HierarchicalPath?.Trim();
            if (!string.IsNullOrEmpty(parentPath))
            {
                parentPath += $"/{form.Id}";
            }
            else
            {
                parentPath = form.Id.ToString();
            }

            var rows = await DbContext.Forms.Where(x => x.HierarchicalPath.StartsWith(parentPath))
                .Select(x => new { x.Id, x.Name, x.HierarchicalPath })
                .ToListAsync();
            if (rows != null && rows.Count > 0)
            {
                return JArray.FromObject(rows);
            }

            return new JArray();
        }

        public async Task<Mwp.Form.Form> GetFormByNameAsync(string formName)
        {
            return await DbContext.Forms.FirstOrDefaultAsync(x => x.Name == formName);
        }

        public async Task<IList<Submission>> GetSubmissionByIds(Guid[] ids)
        {
            return await DbContext.Submissions.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<JToken> ListForm(Guid? tenantId, string searchText, long skip, long take)
        {
            EnsureConnectionOpen();
            var sb = new StringBuilder();
            sb.AppendLine("select count(1)");
            sb.AppendLine("from [mwp].[Forms] with(nolock)");
            sb.Append("where [Name] like @searchText");
            sb.Append(tenantId != null ? " AND TenantId = @tenantId" : " AND TenantId is null");
            sb.AppendLine(";");

            sb.AppendLine("select Id, Name, ISNULL(LastModificationTime,CreationTime) as LastEditTime");
            sb.AppendLine("from [mwp].[Forms] with(nolock)");
            sb.Append("where [Name] like @searchText AND [IsDeleted] = 0");
            if (tenantId != null)
            {
                sb.Append(" AND TenantId = @tenantId");
            }

            sb.AppendLine();
            sb.AppendLine("order by LastModificationTime desc, CreationTime desc");
            sb.AppendLine("OFFSET     @skip ROWS");
            sb.AppendLine("FETCH NEXT @take ROWS ONLY;");
            var query = sb.ToString();
            var result = new JObject();
            var rows = new JArray();
            result.Add("rows", rows);

            var parameters = new Dictionary<string, object>
            {
                { "@searchText", searchText + "%" },
                { "@skip", skip },
                { "@take", take }
            };
            if (tenantId.HasValue)
            {
                parameters.Add("@tenantId", tenantId.Value);
            }

            using (var command = DbContext.Database.CreateQueryCommand(query, parameters))
            {
                using (var rdr = await command.ExecuteReaderAsync())
                {
                    if (rdr.Read())
                    {
                        result.Add("totalCount", rdr.GetInt32(0));
                    }

                    if (rdr.NextResult())
                    {
                        var idxId = rdr.GetOrdinal("Id");
                        var idxName = rdr.GetOrdinal("Name");
                        var idxLastEditTime = rdr.GetOrdinal("LastEditTime");
                        while (rdr.Read())
                        {
                            var row = new JObject();
                            var id = rdr.GetGuid(idxId);
                            var name = rdr.GetString(idxName);
                            var lastEditTs = rdr.GetDateTime(idxLastEditTime);
                            row.Add("Id", id);
                            row.Add("Name", name);
                            row.Add("LastEditTime", lastEditTs);
                            rows.Add(row);
                        }
                    }
                }
            }

            return result;
        }

        public async Task<JToken> ListSubmission(Guid? tenantId, Guid formId, long skip, long take)
        {
            EnsureConnectionOpen();
            var sb = new StringBuilder();
            sb.AppendLine("select count(1) from [mwp].[Submissions] with(nolock)");
            sb.Append("where [FormId] = @formId");
            sb.Append(tenantId != null ? " AND TenantId = @tenantId" : " AND TenantId is NULL");
            sb.AppendLine(";");

            sb.AppendLine("select [Data], ISNULL(LastModificationTime,CreationTime) as LastEditTime");
            sb.AppendLine("from [mwp].[Submissions] with(nolock)");
            sb.Append("where [FormId] = @formId and [IsDeleted] = 0");
            if (tenantId != null)
            {
                sb.Append(" AND TenantId = @tenantId");
            }

            sb.AppendLine();
            sb.AppendLine("order by LastModificationTime desc, CreationTime desc");
            sb.AppendLine("OFFSET     @skip ROWS");
            sb.AppendLine("FETCH NEXT @take ROWS ONLY;");
            var query = sb.ToString();
            var result = new JObject();
            var rows = new JArray();
            result.Add("rows", rows);

            var parameters = new Dictionary<string, object>
            {
                { "@formId", formId },
                { "@skip", skip },
                { "@take", take }
            };
            if (tenantId.HasValue)
            {
                parameters.Add("@tenantId", tenantId.Value.ToString());
            }

            using (var command = DbContext.Database.CreateQueryCommand(query, parameters))
            {
                command.CreateParameter();
                using (var rdr = await command.ExecuteReaderAsync())
                {
                    if (rdr.Read())
                    {
                        result.Add("totalCount", rdr.GetInt32(0));
                    }

                    if (rdr.NextResult())
                    {
                        var idxData = rdr.GetOrdinal("Data");
                        var idxLastEditTime = rdr.GetOrdinal("LastEditTime");
                        while (rdr.Read())
                        {
                            var data = rdr.GetString(idxData);
                            var row = JObject.Parse(data);
                            var lastEditTs = rdr.GetDateTime(idxLastEditTime);
                            row.Add("LastEditTime", lastEditTs);
                            rows.Add(row);
                        }
                    }
                }
            }

            return result;
        }

        private void EnsureConnectionOpen()
        {
            var connection = DbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public Mwp.Form.Form GetSeededRecord(List<Mwp.Form.Form> existingRecords, Mwp.Form.Form record)
        {
            return null;
        }
    }
}