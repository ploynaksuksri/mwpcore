using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Mwp.ExcelIntegration.Form
{
    public class ExcelSubmissionExporter
    {
        public byte[] Export(JObject form, JArray submissions)
        {
            var workbook = new XSSFWorkbook();
            var sheet = workbook.CreateSheet("data");

            InsertHeaderRow(sheet, form);
            InsertSubmissionRows(sheet, form, submissions);

            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                return ms.ToArray();
            }
        }

        private void InsertSubmissionRows(
            ISheet sheet,
            JObject form,
            JArray submissions)
        {
            if (submissions == null || submissions.Count <= 0)
            {
                return;
            }

            var types = new[]
            {
                "textfield",
                "checkbox",
                "number",
                "textarea",
                "url",
                "email",
                "password",
                "phoneNumber"
            };
            var components = form.Value<JArray>("components")
                .Children()
                .Where(x => types.Contains(x.Value<string>("type")))
                .ToList();
            var currentRow = 1;
            submissions.Children().ToList().ForEach(
                r => { currentRow = CreateRowInSheet(sheet, r, components, currentRow); });
        }

        private static int CreateRowInSheet(
            ISheet sheet,
            JToken rowObj,
            IList<JToken> components,
            int currentRow)
        {
            var row = sheet.CreateRow(currentRow);
            currentRow += 1;
            var dataObj = rowObj.Value<JObject>("data");
            for (var i = 0; i < components.Count; i++)
            {
                var field = components[i];
                var fieldType = field.Value<string>("type");
                var fieldKey = field.Value<string>("key");
                if (fieldType == "textfield"
                    || fieldType == "textarea"
                    || fieldType == "url"
                    || fieldType == "email"
                    || fieldType == "password"
                    || fieldType == "phoneNumber")
                {
                    var val = dataObj.Value<string>(fieldKey);
                    row.CreateCell(i, CellType.String).SetCellValue(val);
                }
                else if (fieldType == "checkbox")
                {
                    var val = dataObj.Value<bool>(fieldKey);
                    row.CreateCell(i, CellType.Boolean).SetCellValue(val);
                }
                else if (fieldType == "number")
                {
                    var val = dataObj.Value<double>(fieldKey);
                    row.CreateCell(i, CellType.Numeric).SetCellValue(val);
                }
            }

            return currentRow;
        }

        private void InsertHeaderRow(ISheet sheet, JObject form)
        {
            var headerRow = sheet.CreateRow(0);
            var types = new[]
            {
                "textfield",
                "checkbox",
                "number",
                "textarea",
                "url",
                "email",
                "password",
                "phoneNumber"
            };
            var components = form.Value<JArray>("components")
                .Children()
                .Where(x => types.Contains(x.Value<string>("type")))
                .ToList();
            for (var i = 0; i < components.Count; i++)
            {
                var field = components[i];
                //var fieldType = field.Value<string>("type");
                var fieldKey = field.Value<string>("key");
                headerRow.CreateCell(i).SetCellValue(fieldKey);
            }
        }
    }
}