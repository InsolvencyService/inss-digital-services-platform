using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Pages;
using Reqnroll;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace GovUk.Forms.HostApp.UI.Tests.Helpers;

public static class TestValidator
{
    private static bool _bindingsValidated;
    private static bool _codeValidated;
    private const string DontUseXpath = "Do not use XPath in loactors. Follow best practices.";
    private const string LocatorAccessModifiers = "Locators must be private or protected";

    public static void ValidateScenario(ScenarioContext scenarioContext)
    {
        ValidateScenarioTag(scenarioContext);
        ValidateNoPageObjectsInBindingOnce();

        IEnvironmentConfig environmentConfig = EnvironmentConfigFactory.GetEnvironmentConfig();

        if (!environmentConfig.EnvironmentType.IsAnyOf(TestEnvironment.Prod))
        {
            ValidateCodeOnce();
        }
    }

    private static void ValidateScenarioTag(ScenarioContext scenarioContext)
    {
        string[] tags = scenarioContext.ScenarioInfo.Tags;

        if (tags == null || tags.Length == 0)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must have at least one @tag");
        }
    }

    private static void ValidateNoPageObjectsInBindingOnce()
    {
        if (_bindingsValidated)
        {
            return;
        }

        _bindingsValidated = true;

        IEnumerable<Type> bindingTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttributes<BindingAttribute>().Any());

        foreach (Type bindingType in bindingTypes)
        {
            if (bindingType.HasMemberOrPropertyDerivedFrom<BasePage>())
            {
                throw new InvalidOperationException(
                    $"Binding class '{bindingType.FullName}' must not contain Page Objects. Please use Coordinator classes instead");
            }
        }
    }

    private static void ValidateCodeOnce()
    {
        if (_codeValidated)
        {
            return;
        }

        _codeValidated = true;

        ValidateCodeInternal();
    }


    private static void ValidateCodeInternal([CallerFilePath] string currentFilePath = "")
    {
        string? rootDirectory = TestContext.CurrentContext.WorkDirectory.FindProjectRoot();

        Dictionary<string, string> invalidKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            {"xpath",DontUseXpath },
            {"//*[",DontUseXpath },
            { "//div",DontUseXpath },
            {"/div/",DontUseXpath },
            {"//td",DontUseXpath },
            {"//li",DontUseXpath },
            {"/ul/li",DontUseXpath },
            {"//tr",DontUseXpath },
            {"//span",DontUseXpath },
            {"//a",DontUseXpath },
            {"//h",DontUseXpath },
            {"/body/",DontUseXpath },
            {"[@id=",DontUseXpath },
            {"[@class",DontUseXpath },
            {"[@value",DontUseXpath },
            {"[@text()",DontUseXpath },
            {"[contain",DontUseXpath },
            {"[contain",DontUseXpath },
            {"public ilocator",LocatorAccessModifiers },
            {"internal ilocator",LocatorAccessModifiers },
        };

        Regex regex = new(
            string.Join("|", invalidKeywords.Keys.Select(Regex.Escape)),
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        HashSet<string> ignores = new(StringComparer.OrdinalIgnoreCase)
        {
            "bin", "obj", ".git", ".vs"
        };

        foreach (string? file in rootDirectory!
                     .FindAllFilePaths(".cs", ignores)
                     .Where(f => !string.Equals(f, currentFilePath, StringComparison.OrdinalIgnoreCase)))
        {
            int lineNum = 0;

            foreach (string? line in File.ReadLines(file))
            {
                lineNum++;

                if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                Match match = regex.Match(line);

                if (match.Success)
                {
                    throw new InvalidOperationException(
                        $"Invalid keyword '{match.Value}' in {file} (line {lineNum})\n" +
                        invalidKeywords[match.Value]);
                }
            }
        }
    }
}
