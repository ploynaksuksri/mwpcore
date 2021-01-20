namespace Mwp.Form
{
    public class FormIoProps
    {
        public const string ParentFormId = "parentFormId";
        public const string MergeData = "mergeData";
        public const string CurrentVersion = "currentVersion";
        public const string Timestamp = "timestamp";
        public const string MwpMetaData = "mwp_metadata";
        public const string ObjId = "_id";
        public const string Name = "name";
        public const string UserId = "userId";
        public const string Form = "form";
        public const string ExternalIds = "externalIds";
        public const string Data = "data";
        public const string Reference = "reference";
        public const string Key = "key";
        public const string Id = "id";
        public const string Parent = "parent";
        public const string ObjType = "type";
        public const string Components = "components";
        public const string Label = "label";
        public const string Mwp = "mwp";

        public class FileProps
        {
            public const string FileId = "fileId";
            public const string FileName = "fileName";
            public const string FileSize = "fileSize";
            public const string Sha1 = "sha1";
            public const string IsFromTemplate = "isFromTemplate";
        }

        public class MwpProps
        {
            public const string ParentFormId = "parentFormId";
            public const string ParentFormVersion = "parentFormVersion";
            public const string ParentSubmissionId = "parentSubmissionId";
            public const string ParentSubmissionVersion = "parentSubmissionVersion";
            public const string HierarchicalPath = "hierarchical_path";

            public const string ParentNodes = "parent_nodes";
        }

        public class ParentNodeProps
        {
            public const string Id = "id";
            public const string Version = "version";
        }

        public const string IncludeInTheRollOver = "includeInTheRollOver";
    }
}