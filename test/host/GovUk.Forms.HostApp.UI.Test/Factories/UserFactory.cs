using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;


namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class UserFactory
{
    private const string SharedPassword = "Agresso!17";

    private static readonly Dictionary<string, string> _users =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["InssTestOne"] = "insstestone@gmail.com",
            ["InssTestTwo"] = "insstesttwo@gmail.com",
            ["InssTestThree"] = "insstestthree@gmail.com",
            ["InssTestFour"] = "insstestfour@gmail.com",
            ["InssTestFive"] = "insstestfive@gmail.com",
            ["InssTestSix"] = "insstestsix@gmail.com",
            ["InssTestSeven"] = "insstestseven@gmail.com",
            ["InssTestEight"] = "insstesteight@gmail.com",
            ["InssTestNine"] = "insstestnine@gmail.com",
            ["InssTestTen"] = "insstestten@gmail.com",
            ["InssTestEleven"] = "insstesteleven@gmail.com",
            ["InssTestTwelve"] = "insstesttwelve@gmail.com",
            ["InssTestThirteen"] = "insstestthirteen@gmail.com",
            ["InssTestFourteen"] = "insstestfourteen@gmail.com",
            ["InssTestFifteen"] = "insstestfifteen@gmail.com",
            ["InssTestSixteen"] = "insstestsixteen@gmail.com",
            ["InssTestSeventeen"] = "insstestseventeen@gmail.com",
            ["InssTestManOne"] = "insstestmanone@gmail.com",
            ["InssTestManTwo"] = "insstestmantwo@gmail.com"
        };

    public static TestUser DefaultUser() => new()
    {
        Email = ScenarioConstant.EmailAddress,
        Password = ScenarioConstant.Password
    };

    public static TestUser GetUser(string userType)
    {
        if (string.Equals(userType, "Default", StringComparison.OrdinalIgnoreCase))
        {
            return DefaultUser();
        }

        if (!_users.TryGetValue(userType, out string? email))
        {
            throw new ArgumentException($"Unknown user type: {userType}", nameof(userType));
        }

        return new TestUser
        {
            Email = email,
            Password = SharedPassword
        };
    }
}
