namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvDevConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Dev;
    public override string BaseUrl => "https://dev-forms-hostapp.azurewebsites.net";
}
