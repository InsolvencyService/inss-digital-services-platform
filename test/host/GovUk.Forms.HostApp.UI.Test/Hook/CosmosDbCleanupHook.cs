using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Hook;

[Binding]
public sealed class CosmosDbCleanupHook
{
    private const string UserTypeKey = "UserType";

    private readonly ICosmosDbService _cosmosDbService;
    private readonly ScenarioContext _scenarioContext;

    public CosmosDbCleanupHook(
        ICosmosDbService cosmosDbService,
        ScenarioContext scenarioContext)
    {
        _cosmosDbService = cosmosDbService;
        _scenarioContext = scenarioContext;
    }

    [AfterScenario("cleanCosmosDb", Order = -1)]
    public async Task CleanAfterScenarioAsync()
    {
        if (!_scenarioContext.TryGetValue(UserTypeKey, out string? userType) ||
            string.IsNullOrWhiteSpace(userType))
        {
            return;
        }

        string email = UserFactory.GetUser(userType).Email;

        if (string.IsNullOrWhiteSpace(email))
        {
            return;
        }

        await _cosmosDbService.DeleteIpUploadAsync(email);
        await _cosmosDbService.DeleteDynamicsByUserIdAsync(email);
    }
}