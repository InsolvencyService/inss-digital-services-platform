namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvLocalConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.Local;
    public override string BaseUrl => "http://localhost:5000";
}
