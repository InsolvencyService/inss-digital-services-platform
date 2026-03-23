using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

[CollectionDefinition(Name)]
public sealed class WebApplicationFactoryFixture : ICollectionFixture<TestWebApplicationFactory>
{
    public const string Name = "End-to-End";
}