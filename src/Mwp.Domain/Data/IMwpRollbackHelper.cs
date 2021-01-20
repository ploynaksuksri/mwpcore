namespace Mwp.Data
{
    public interface IMwpRollbackHelper
    {
        string GetRollbackScript(string rollbackFileName);
    }
}