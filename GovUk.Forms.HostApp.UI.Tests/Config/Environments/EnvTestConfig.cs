namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvTestConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Test;
    public override string BaseUrl => "https://test-forms-hostapp.azurewebsites.net";
}
