using System;

namespace Mwp.File.Events
{
    [Serializable]
    public class UploadedFileEventData
    {
        public UploadFileResult UploadResult { get; private set; }

        public UploadedFileEventData(UploadFileResult uploadResult)
        {
            UploadResult = uploadResult;
        }
    }
}