using System;

namespace Mwp.Form.Dtos
{
    public class ImportSubmissionResult
    {
        public bool Success { get; set; }

        public Guid[] ImportedSubmissionIds { get; set; }

        public string Message { get; set; }
    }
}