using System;

namespace Mwp.Form.Dtos
{
    public class FormRolloverInfoDto
    {
        public Guid FormId { get; set; }
        public string NewFormName { get; set; }
        public bool IsRolloverSubmission { get; set; }
        public bool IsPreserveParentChanges { get; set; }
        public bool IsForceRolloverAllComponents { get; set; }
    }
}