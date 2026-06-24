namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvDevConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Dev;
    public override string BaseUrl => "https://localhost:5056/";
    public override string CosmosEndpoint => "https://localhost:8081";
}
