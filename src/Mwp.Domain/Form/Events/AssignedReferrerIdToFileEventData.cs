using System;
using Mwp.File;

namespace Mwp.Form.Events
{
    [Serializable]
    public class AssignedReferrerIdToFileEventData
    {
        public Guid ReferrerId { get; private set; }
        public string[] FileIds { get; private set; }
        public FileReferrerType ReferrerType { get; set; }

        public AssignedReferrerIdToFileEventData(Guid referrerId, string[] fileIds, FileReferrerType referrerType = FileReferrerType.Submission)
        {
            ReferrerId = referrerId;
            FileIds = fileIds;
            ReferrerType = referrerType;
        }
    }
}