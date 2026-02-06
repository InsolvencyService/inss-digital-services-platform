using FluentAssertions;
using FluentAssertions.Execution;
using INSS.Platform.Cache.Infrastructure.Configuration;
using INSS.Platform.Cache.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;

namespace INSS.Platform.Cache.Infrastructure.Tests;

public class CacheRepositoryTests
{
    private readonly Mock<ILogger<CacheRepository>> _loggerMock = new ();
    private readonly Mock<CosmosClient> _cosmosClientMock = new ();
    private readonly Mock<IOptions<CosmosCacheOptions>> _optionsMock = new ();
    private readonly Mock<Database> _databaseMock = new ();
    private readonly Mock<Container> _containerMock = new ();

    [Fact]
    public async Task GetFormAsync_ReturnsForm_WhenFound()
    {
        // Arrange
        CacheRepository repository = CreateRepository();
        Guid id = Guid.NewGuid();
        TestForm expectedForm = new() { Id = id, Name = "TestForm" };
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(expectedForm);
        List<dynamic> items = new() { JObject.Parse(json) };

        Mock<FeedIterator<dynamic>> feedIteratorMock = new();
        feedIteratorMock.SetupSequence(x => x.HasMoreResults).Returns(true).Returns(false);
        feedIteratorMock.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeedResponseStub<dynamic>(items));

        _containerMock.Setup(x => x.GetItemQueryIterator<dynamic>(It.IsAny<QueryDefinition>(), null, null))
            .Returns(feedIteratorMock.Object);

        // Act
        TestForm? result = await repository.GetFormAsync<TestForm>(id);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.Name.Should().Be("TestForm");
        }
    }

    [Fact]
    public async Task GetFormAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        CacheRepository repository = CreateRepository();
        Guid id = Guid.NewGuid();
        List<dynamic> items = new();

        Mock<FeedIterator<dynamic>> feedIteratorMock = new();
        feedIteratorMock.Setup(x => x.HasMoreResults).Returns(true);
        feedIteratorMock.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeedResponseStub<dynamic>(items));

        _containerMock.Setup(x => x.GetItemQueryIterator<dynamic>(It.IsAny<QueryDefinition>(), null, null))
            .Returns(feedIteratorMock.Object);

        // Act
        TestForm? result = await repository.GetFormAsync<TestForm>(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetFormAsync_LogsAndReturnsNull_OnException()
    {
        // Arrange
        CacheRepository repository = CreateRepository();
        Guid id = Guid.NewGuid();
        _containerMock.Setup(x => x.GetItemQueryIterator<dynamic>(It.IsAny<QueryDefinition>(), null, null))
            .Throws(new CosmosException("error", HttpStatusCode.InternalServerError, 0, string.Empty, 0));

        // Act
        TestForm? result = await repository.GetFormAsync<TestForm>(id);

        // Assert
        result.Should().BeNull();
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task SaveFormAsync_ReturnsStatusCode_OnSuccess()
    {
        // Arrange
        CacheRepository repository = CreateRepository();
        TestForm form = new() { Id = Guid.NewGuid(), FormSetId = Guid.NewGuid(), Name = "TestForm" };
        JObject jObject = JObject.FromObject(form);
        ItemResponse<JObject> response = Mock.Of<ItemResponse<JObject>>(r => r.StatusCode == HttpStatusCode.OK);

        _containerMock.Setup(x => x.UpsertItemAsync(
            It.IsAny<JObject>(),
            It.IsAny<PartitionKey>(),
            null,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        HttpStatusCode statusCode = await repository.SaveFormAsync(form);

        // Assert
        statusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveFormAsync_ReturnsBadRequest_OnException()
    {
        // Arrange
        CacheRepository repository = CreateRepository();
        TestForm form = new() { Id = Guid.NewGuid(), FormSetId = Guid.NewGuid(), Name = "TestForm" };
        _containerMock.Setup(x => x.UpsertItemAsync(
            It.IsAny<JObject>(),
            It.IsAny<PartitionKey>(),
            null,
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new CosmosException("error", HttpStatusCode.InternalServerError, 0, string.Empty, 0));

        // Act
        HttpStatusCode statusCode = await repository.SaveFormAsync(form);

        // Assert
        statusCode.Should().Be(HttpStatusCode.BadRequest);
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            Times.AtLeastOnce);
    }

    private CacheRepository CreateRepository()
    {
        CosmosCacheOptions options = new() { Database = "Forms", Container = "Form" };

        _optionsMock.Setup(x => x.Value).Returns(options);
        _cosmosClientMock.Setup(x => x.GetDatabase(It.IsAny<string>())).Returns(_databaseMock.Object);
        _databaseMock.Setup(x => x.GetContainer(It.IsAny<string>())).Returns(_containerMock.Object);

        return new CacheRepository(_loggerMock.Object, _cosmosClientMock.Object, _optionsMock.Object);
    }
}
