using Intuit.Ipp.Data;
using Mwp.Financials.Reports;
using Mwp.Qbo;

namespace Mwp.Reports
{
    public static class TrialBalanceReportMapper
    {
        public static TrialBalanceReport Map(Report qboReport)
        {
            var report = new TrialBalanceReport
            {
                Name = qboReport.Header.ReportName,
                ReportDate = qboReport.Header.Time.ToString("D")
            };

            foreach (var row in qboReport.Rows)
            {
                if (row.group == QboConsts.GrandTotalRecord)
                {
                    var summary = ((Summary)row.AnyIntuitObjects[0]).ColData;

                    report.Summary.Debit = summary[1].value;
                    report.Summary.Credit = summary[2].value;
                    continue;
                }

                var rowData = (ColData[])row.AnyIntuitObjects[0];
                report.Records.Add(new TrialBalanceRecord
                {
                    AccountCode = rowData[0].id,
                    AccountName = rowData[0].value,
                    Debit = rowData[1].value,
                    Credit = rowData[2].value
                });
            }

            return report;
        }
    }
}