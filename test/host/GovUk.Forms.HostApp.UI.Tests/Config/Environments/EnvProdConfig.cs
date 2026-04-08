namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvProdConfig : IEnvironmentConfig
{
    public TestEnvironment EnvironmentType => TestEnvironment.Prod;
    public string BaseUrl => "https://forms.service.gov.uk";
}
