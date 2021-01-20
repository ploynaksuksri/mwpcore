using System;
using System.Collections.Generic;

namespace Mwp.File.Events
{
    [Serializable]
    public class DeletedFileIndexEventData
    {
        public List<string> FileIds { get; private set; }

        public DeletedFileIndexEventData(List<string> fileIds)
        {
            FileIds = fileIds;
        }
    }
}