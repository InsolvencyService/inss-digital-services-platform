namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public class EnvQaConfig : BaseEnvironmentConfig
{
    public override TestEnvironment EnvironmentType => TestEnvironment.QA;
    public override string BaseUrl => "http://localhost:5000";
}
