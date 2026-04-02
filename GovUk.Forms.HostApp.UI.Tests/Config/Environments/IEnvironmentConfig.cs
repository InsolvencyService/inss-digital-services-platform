namespace GovUk.Forms.HostApp.UI.Tests.Config.Environments;

public interface IEnvironmentConfig
{
    TestEnvironment EnvironmentType { get; }
    string BaseUrl { get; }
}
