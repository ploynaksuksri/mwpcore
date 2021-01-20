using System;

namespace Mwp.File.Events
{
    [Serializable]
    public class DeletedFileEventData
    {
        public string FileId { get; private set; }

        public DeletedFileEventData(string fileId)
        {
            FileId = fileId;
        }
    }
}