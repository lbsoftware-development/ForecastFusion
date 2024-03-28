using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ForecastFusion.Application.Contracts;

namespace ForecastFusion.Application.Services
{
    public class AzureKeyVaultService: IAzureKeyVaultService
    {
        private const string KEY_VAULT_NAME = "forecastfusionkeyvault";
        private readonly SecretClient secretClient;
        public AzureKeyVaultService()
        {
            secretClient = new SecretClient(new Uri($"https://{KEY_VAULT_NAME}.vault.azure.net/"), new DefaultAzureCredential());
        }

        public AzureKeyVaultService(SecretClient secretClient)
        {
            this.secretClient = secretClient;
        }

        public async Task<string> GetSecretFromVault(string secretName)
        {
            if (string.IsNullOrEmpty(secretName))
            {
                throw new ArgumentException($"{nameof(secretName)} parameter cannot be null or empty", secretName);
            }

            try
            {
                var secret = await secretClient.GetSecretAsync(secretName);
                return secret?.Value?.Value?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                //TODO add logging/error handling}
                return string.Empty;
            }            
        }
    }
}
