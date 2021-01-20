namespace Mwp.Wopi
{
    public enum WopiRequestType
    {
        None,
        CheckFileInfo,
        GetFile,
        Lock,
        GetLock,
        RefreshLock,
        Unlock,
        UnlockAndRelock,
        PutFile,
        PutRelativeFile,
        RenameFile,
        DeleteFile
    }
}