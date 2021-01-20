namespace Mwp.Wopi
{
    /// <summary>
    ///     Contains valid WOPI request headers
    /// </summary>
    public class WopiRequestHeader
    {
        //WOPI Header Consts
        public const string APP_ENDPOINT = "X-WOPI-AppEndpoint";
        public const string CLIENT_VERSION = "X-WOPI-ClientVersion";
        public const string CORRELATION_ID = "X-WOPI-CorrelationId";
        public const string LOCK = "X-WOPI-Lock";
        public const string MACHINE_NAME = "X-WOPI-MachineName";
        public const string MAX_EXPECTED_SIZE = "X-WOPI-MaxExpectedSize";
        public const string OLD_LOCK = "X-WOPI-OldLock";
        public const string OVERRIDE = "X-WOPI-Override";
        public const string OVERWRITE_RELATIVE_TARGET = "X-WOPI-OverwriteRelativeTarget";
        public const string VALID_RELATIVE_TARGET = "X-WOPI-ValidRelativeTarget";
        public const string PREF_TRACE_REQUESTED = "X-WOPI-PerfTraceRequested";
        public const string PROOF = "X-WOPI-Proof";
        public const string PROOF_OLD = "X-WOPI-ProofOld";
        public const string RELATIVE_TARGET = "X-WOPI-RelativeTarget";
        public const string REQUESTED_NAME = "X-WOPI-RequestedName";
        public const string SESSION_CONTEXT = "X-WOPI-SessionContext";
        public const string SESSION_ID = "X-WOPI-SessionId";
        public const string SIZE = "X-WOPI-Size";
        public const string SUGGESTED_TARGET = "X-WOPI-SuggestedTarget";
        public const string TIME_STAMP = "X-WOPI-TimeStamp";
        public const string EDITORS = "X-WOPI-Editors";
    }
}