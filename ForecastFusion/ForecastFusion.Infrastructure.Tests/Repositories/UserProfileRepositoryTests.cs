using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using ForecastFusion.Infrastructure.Repositories;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Infrastructure.Entities;
using ForecastFusion.Infrastructure.Mappings;
using ForecastFusion.Infrastructure.Const;
using DomainEntities = ForecastFusion.Domain.Entities;
using System.Net;
using Azure.Data.Tables;
using ForecastFusion.Application;
using Azure;
using System.Diagnostics.Metrics;


namespace ForecastFusion.Infrastructure.Tests.Repositories
{
    public class UserProfileRepositoryTests
    {
        private Mock<IAzureTableStorageService> _azureTableStorageServiceMock;
        private UserProfileRepository _userProfileRepository;

        [SetUp]
        public void Setup()
        {
            _azureTableStorageServiceMock = new Mock<IAzureTableStorageService>();
            _userProfileRepository = new UserProfileRepository(_azureTableStorageServiceMock.Object);
        }

        [Test]
        public async Task GetUserProfileAsync_Success_ReturnsUserProfile()
        {
            // Arrange
            var country = "TestCountry";
            var userId = "TestUserId";
            var userProfileEntity = new UserProfile() { Name = "TestName", Location = "TestLocation" };
            var userProfile = new DomainEntities.UserProfile() { Country = country, Location = "TestLocation", Name = "TestName" };
            
            _azureTableStorageServiceMock.Setup(x => x.RetrieveEntityAsync<ITableEntity>(AzureStorageTableNames.USER_PROFILE, country, userId))
            .ReturnsAsync(Result<ITableEntity>.Success(userProfileEntity));

            // Act
            var result = await _userProfileRepository.GetUserProfileAsync(country, userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(userProfile, result.Value);
        }

        [Test]
        public async Task GetUserProfileAsync_Failure_ReturnsFailureResult()
        {
            // Arrange
            var country = "TestCountry";
            var userId = "TestUserId";
            var error = "Error message";
            
            _azureTableStorageServiceMock.Setup(x => x.RetrieveEntityAsync<ITableEntity>(AzureStorageTableNames.USER_PROFILE, country, userId))
                .ReturnsAsync(Result<ITableEntity>.Failure(new RequestFailedException("Error")));

            // Act
            var result = await _userProfileRepository.GetUserProfileAsync(country, userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpStatusCode);
            Assert.AreEqual(error, result.Error);
        }

        [Test]
        public async Task UpsertUserProfileAsync_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userProfileEntity = new UserProfile() { Name = "TestName", Location = "TestLocation" };
            
            _azureTableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<ITableEntity>(), AzureStorageTableNames.USER_PROFILE))
                .ReturnsAsync(Result.Success(HttpStatusCode.OK));

            // Act
            var result = await _userProfileRepository.UpsertUserProfileAsync(userProfile);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async Task UpsertUserProfileAsync_Failure_ReturnsFailureResult()
        {
            // Arrange
            var userProfile = new DomainEntities.UserProfile();
            var error = "Error message";

            _azureTableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<UserProfile>(), AzureStorageTableNames.USER_PROFILE))
                .ReturnsAsync(Result.Failure(new RequestFailedException(error, 500, null)));

            // Act
            var result = await _userProfileRepository.UpsertUserProfileAsync(userProfile);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.HttpStatusCode);
            Assert.AreEqual(error, result.Error);
        }
    }
}

