namespace ForecastFusion.Application.Contracts
{
    public interface IAzureKeyVaultService
    {
        public Task<string> GetSecretFromVault(string secretName);
    }
}
