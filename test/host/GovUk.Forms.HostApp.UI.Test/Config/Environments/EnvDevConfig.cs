namespace GovUk.Forms.HostApp.UI.Test.Config.Environments;

public class EnvDevConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Dev;
    public override string BaseUrl => "https://dev.ipus.redundancy-payments.service.gov.uk/";
    public override string CosmosEndpoint => "https://cosmos-platform-uksouth-sit.documents.azure.com:443";
}
