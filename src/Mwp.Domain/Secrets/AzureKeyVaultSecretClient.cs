namespace Mwp.Secrets
{
    public class AzureKeyVaultSecretClient
    {
        public static string GetKeyVaultUri(string keyVaultName)
        {
            return $"https://{keyVaultName}.vault.azure.net/";
        }
    }
}