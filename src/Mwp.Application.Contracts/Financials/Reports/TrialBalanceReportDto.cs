using System;
using System.Collections.Generic;

namespace Mwp.Financials.Reports
{
    [Serializable]
    public class TrialBalanceReportDto
    {
        public string Name { get; set; }
        public string ReportDate { get; set; }

        public List<string> Headers { get; set; } = new List<string>
        {
            "Account Code", "Account", "Account Type", "Debit", "Credit"
        };

        public List<TrialBalanceRecordDto> Records { get; set; } = new List<TrialBalanceRecordDto>();

        public TrialBalanceRecordDto Summary { get; set; }
    }
}