namespace ForecastFusion.Application.Tests;

using Azure;
using Azure.Security.KeyVault.Secrets;
using ForecastFusion.Application.Services;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;


[TestFixture]
public class AzureKeyVaultServiceTests
{
    private AzureKeyVaultService azureKeyVaultService;
    private Mock<SecretClient> secretClientMock;

    [SetUp]
    public void SetUp()
    {
        secretClientMock = new Mock<SecretClient>(MockBehavior.Strict);
        azureKeyVaultService = new AzureKeyVaultService(secretClientMock.Object);
    }

    [Test]
    public async Task GetSecretFromVault_ValidSecretName_ReturnsSecretValue()
    {
        // Arrange
        const string secretName = "validSecret";
        const string secretValue = "supersecret";
        
        var mockResponse = new Mock<Response<KeyVaultSecret>>();        
        var secret = new KeyVaultSecret(secretName, secretValue);

        mockResponse.SetupGet(r => r.Value).Returns(secret);

        secretClientMock
        .Setup(x => x.GetSecretAsync(secretName, default, default))
        .ReturnsAsync(mockResponse.Object);

        // Act
        var result = await azureKeyVaultService.GetSecretFromVault(secretName);

        // Assert
        
        Assert.That(result, Is.EqualTo(secretValue));        
    }

    [Test]
    public void GetSecretFromVault_NullOrEmptySecretName_ThrowsArgumentException()
    {
        // Arrange
        const string emptySecretName = "";        

        // Assert & Act
        Assert.ThrowsAsync<ArgumentException>(async () => await azureKeyVaultService.GetSecretFromVault(emptySecretName));
    }

    [Test]
    public async Task GetSecretFromVault_SecretClientThrowsException_ReturnsEmptyString()
    {
        // Arrange
        const string secretName = "validSecret";

        var mockResponse = new Mock<Response<KeyVaultSecret>>();        

        secretClientMock.Setup(x => x.GetSecretAsync(secretName, default, default)).ThrowsAsync(new Azure.RequestFailedException("Simulated failure"));

        // Act
        var result = await azureKeyVaultService.GetSecretFromVault(secretName);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }
}

