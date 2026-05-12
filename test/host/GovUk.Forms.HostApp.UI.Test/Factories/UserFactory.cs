using Bogus;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;


namespace GovUk.Forms.HostApp.UI.Test.Factories;

/// <summary>
/// Temporary solution for providing test users while authentication and
/// authorisation are not fully implemented.
///
/// This will be removed or replaced once the database-backed user
/// management is available.
/// </summary>
public static class UserFactory
{
    public static TestUser DefaultUser() => new()
    {
        Email = ScenarioConstant.EmailAddress,
        Password = ScenarioConstant.Password
    };

    public static TestUser CreateRandomUser() => new()
    {
        Email = new Faker().Internet.Email(),
        Password = "Password123"
    };

    public static TestUser GetUser(string type) => type switch
    {
        "Admin" => CreateRandomUser(),
        "Default" => DefaultUser(),
        _ => throw new ArgumentException($"Unknown user type: {type}")
    };
}
