using System;
using System.Linq;
using Microsoft.Extensions.Localization;
using Mwp.Financials.Reports;
using Mwp.Localization;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Mwp.Xero.Reports
{
    public class TrialBalanceReportMapper
    {
        protected static IStringLocalizer<MwpResource> _l;

        public TrialBalanceReportMapper(IStringLocalizer<MwpResource> l)
        {
            _l = l;
        }

        public static TrialBalanceReport Map(ReportWithRow xeroReport)
        {
            var report = new TrialBalanceReport
            {
                Name = $"{xeroReport.ReportTitles[0]} - {xeroReport.ReportTitles[1]}",
                ReportDate = xeroReport.ReportDate
            };

            foreach (var section in xeroReport.Rows.Where(e => e.RowType == RowType.Section))
            {
                var title = section.Title;

                foreach (var row in section.Rows)
                {
                    var cells = row.Cells;

                    switch (row.RowType)
                    {
                        case RowType.Row:
                        {
                            var accountName = cells[0].Value;

                            var record = new TrialBalanceRecord
                            {
                                AccountName = accountName.Split("(")[0].TrimEnd(),
                                AccountCode = accountName.Split("(")[1].TrimEnd(')'),
                                AccountType = title,
                                Debit = cells[1].Value,
                                Credit = cells[2].Value
                            };

                            if (title.Equals("Assets") || title.Equals("Equity") || title.Equals("Liabilities"))
                            {
                                record.Debit = cells[3].Value;
                                record.Credit = cells[4].Value;
                            }

                            report.Records.Add(record);
                            break;
                        }
                        case RowType.SummaryRow:
                            report.Summary = new TrialBalanceRecord
                            {
                                AccountCode = _l["Total"],
                                Debit = cells[3].Value,
                                Credit = cells[4].Value
                            };
                            break;
                        case RowType.Header:
                            break;
                        case RowType.Section:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            report.Records = report.Records.OrderBy(e => e.AccountCode).ToList();
            return report;
        }
    }
}