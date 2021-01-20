using System;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NPOI.XSSF.UserModel;

namespace Mwp.ExcelIntegration.Form
{
    public class ExcelSubmissionImporter
    {
        public JArray ImportSubmissions(JObject formJObj, byte[] data)
        {
            var importedRows = new JArray();
            var dt = ReadExcelAsDataTable(data);
            foreach (DataRow row in dt.Rows)
            {
                var importRow = CreateNewSubmissionAsJObject(formJObj);
                AssignSubmissionData(row, importRow);
                importedRows.Add(importRow);
            }

            return importedRows;
        }

        private void AssignSubmissionData(DataRow srcRow, JObject targetObj)
        {
            var columns = srcRow.Table.Columns;
            var dataObj = targetObj.Value<JObject>("data");
            if (dataObj == null)
            {
                return;
            }

            foreach (DataColumn column in columns)
            {
                var colName = column.ColumnName;
                if (dataObj.ContainsKey(colName))
                {
                    if (dataObj[colName]?.Type == JTokenType.String)
                    {
                        dataObj[colName] = srcRow[colName].ToString();
                    }
                    else if (dataObj[colName]?.Type == JTokenType.Integer || dataObj[colName]?.Type == JTokenType.Float)
                    {
                        dataObj[colName] = float.Parse(srcRow[colName]?.ToString() ?? "0");
                    }
                    else if (dataObj[colName]?.Type == JTokenType.Boolean)
                    {
                        dataObj[colName] = bool.Parse(srcRow[colName]?.ToString()?.ToLower() ?? "false");
                    }
                }
            }
        }

        private JObject CreateNewSubmissionAsJObject(JObject formJObj)
        {
            var obj = new JObject();
            obj["data"] = new JObject();
            obj["state"] = "submitted";
            obj["_id"] = Guid.NewGuid();
            obj["form"] = formJObj.Value<string>("_id");
            obj["externalIds"] = new JArray();
            AssignDefaultValues(formJObj, obj);
            return obj;
        }

        private void AssignDefaultValues(JObject formJObj, JObject obj)
        {
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
            var dataObj = obj.Value<JObject>("data");
            formJObj.Value<JArray>("components")
                .Children()
                .Where(x => types.Contains(x.Value<string>("type")))
                .ToList()
                .ForEach(x =>
                {
                    var fieldType = x.Value<string>("type");
                    var fieldKey = x.Value<string>("key");
                    if (fieldType == "number")
                    {
                        dataObj[fieldKey] = 0;
                    }
                    else if (fieldType == "checkbox")
                    {
                        dataObj[fieldKey] = false;
                    }
                    else if (fieldType == "textfield"
                             || fieldType == "textarea"
                             || fieldType == "url"
                             || fieldType == "email"
                             || fieldType == "password"
                             || fieldType == "phoneNumber")
                    {
                        dataObj[fieldKey] = "";
                    }
                });
        }

        private DataTable ReadExcelAsDataTable(byte[] data)
        {
            var workbook = CreateWorkBookFromData(data);
            // Read the current table data
            var sheet = (XSSFSheet)workbook.GetSheetAt(0);
            // Read the current row data
            var headerRow = (XSSFRow)sheet.GetRow(0);
            // LastCellNum is the number of cells of current rows
            var cellCount = headerRow.LastCellNum;
            var dt = new DataTable();
            try
            {
                if (dt.Rows.Count == 0)
                {
                    //Reading First Row as Header for Excel Sheet;
                    try
                    {
                        for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                        {
                            // get data as the column header of DataTable
                            var column = new DataColumn(headerRow.GetCell(j).StringCellValue);
                            dt.Columns.Add(column);
                        }
                    }
                    catch (Exception)
                    {
                        // NOOP
                    }
                }

                for (var sheetindex = 0; sheetindex < workbook.NumberOfSheets; sheetindex++)
                {
                    sheet = (XSSFSheet)workbook.GetSheetAt(sheetindex);
                    if (null != sheet)
                    {
                        // LastRowNum is the number of rows of current table
                        var rowCount = sheet.LastRowNum + 1;
                        //Reading Rows and Copying it to Data Table;
                        try
                        {
                            for (var i = sheet.FirstRowNum + 1; i < rowCount; i++)
                            {
                                var row = (XSSFRow)sheet.GetRow(i);
                                var dataRow = dt.NewRow();
                                var isBlankRow = true;
                                try
                                {
                                    for (int j = row.FirstCellNum; j < cellCount; j++)
                                    {
                                        if (null != row.GetCell(j)
                                            && !string.IsNullOrEmpty(row.GetCell(j).ToString())
                                            && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                                        {
                                            dataRow[j] = row.GetCell(j).ToString();
                                            isBlankRow = false;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    // NOOP
                                }

                                if (!isBlankRow)
                                {
                                    dt.Rows.Add(dataRow);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // NOOP
                        }
                    }
                }
            }
            catch (Exception)
            {
                // NOOP
            }
            finally
            {
                workbook.UnlockStructure();
                workbook.UnlockRevision();
                workbook.UnlockWindows();
            }

            return dt;
        }

        private XSSFWorkbook CreateWorkBookFromData(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                return new XSSFWorkbook(ms);
            }
        }
    }
}