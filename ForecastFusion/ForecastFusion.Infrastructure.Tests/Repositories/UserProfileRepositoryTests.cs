using Azure;
using Azure.Data.Tables;
using ForecastFusion.Application;
using ForecastFusion.Application.Contracts;
using ForecastFusion.Infrastructure.Const;
using ForecastFusion.Infrastructure.Entities;
using ForecastFusion.Infrastructure.Repositories;
using Moq;
using System.Net;
using DomainEntities = ForecastFusion.Domain.Entities;


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
            var userId = Guid.NewGuid().ToString();
            var userProfileEntity = new UserProfile() { PartitionKey = country, RowKey = userId, Name = "TestName", Location = "TestLocation" };
            var userProfile = new DomainEntities.UserProfile() { Country = country, Location = "TestLocation", Name = "TestName", Id = Guid.Parse(userId) };
            
            _azureTableStorageServiceMock.Setup(x => x.RetrieveEntityAsync<ITableEntity>(AzureStorageTableNames.USER_PROFILE, country, userId))
            .ReturnsAsync(Result<ITableEntity>.Success(userProfileEntity));

            // Act
            var result = await _userProfileRepository.GetUserProfileAsync(country, userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.That(result.Value.Country, Is.EqualTo(userProfile.Country));
            Assert.That(result.Value.Name, Is.EqualTo(userProfile.Name));
            Assert.That(result.Value.EmailAddress, Is.EqualTo(userProfile.EmailAddress));
            Assert.That(result.Value.Id, Is.EqualTo(userProfile.Id));
            Assert.That(result.Value.Location, Is.EqualTo(userProfile.Location));            
        }

        [Test]
        public async Task GetUserProfileAsync_Failure_ReturnsFailureResult()
        {
            // Arrange
            var country = "TestCountry";
            var userId = "TestUserId";
            var error = "Error message";
            var mockResult = Result<ITableEntity>.Failure(new RequestFailedException(error));
            mockResult.HttpStatusCode = HttpStatusCode.NotFound;


            _azureTableStorageServiceMock.Setup(x => x.RetrieveEntityAsync<ITableEntity>(AzureStorageTableNames.USER_PROFILE, country, userId))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _userProfileRepository.GetUserProfileAsync(country, userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(result.Error.Message, Is.EqualTo(error));
        }

        [Test]
        public async Task UpsertUserProfileAsync_Success_ReturnsSuccessResult()
        {
            // Arrange
            var userProfile = new DomainEntities.UserProfile { Name = "TestName", Location = "TestLocation", Country = "England" };
            
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
            var userProfile = new DomainEntities.UserProfile { Name = "TestName", Location = "TestLocation", Country = "England" };
            var error = "Error message";
            var mockResult = Result.Failure(new RequestFailedException(error));
            mockResult.HttpStatusCode = HttpStatusCode.InternalServerError;

            _azureTableStorageServiceMock.Setup(x => x.UpsertEntityAsync(It.IsAny<UserProfile>(), AzureStorageTableNames.USER_PROFILE))
                .ReturnsAsync(mockResult);

            // Act
            var result = await _userProfileRepository.UpsertUserProfileAsync(userProfile);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(result.Error.Message, Is.EqualTo(error));
        }
    }
}

