namespace Mwp.Financials.Reports
{
    public class TrialBalanceRecordDto
    {
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string YTDDebit { get; set; }
        public string YTDCredit { get; set; }
        public string RecordType { get; set; }
    }
}