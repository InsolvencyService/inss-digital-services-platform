using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Pages;
using GovUk.Forms.HostApp.UI.Tests.Tags;
using Microsoft.Extensions.DependencyInjection;
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

        IEnvironmentConfig environmentConfig = EnvironmentConfigFactory.EnvironmentConfig;

        if (!environmentConfig.EnvironmentType.IsAnyOf(TestEnvironment.Prod))
        {
            ValidateCodeOnce();
        }

        ValidateAllPagesRegistered();
    }

    private static void ValidateScenarioTag(ScenarioContext scenarioContext)
    {
        string[] tags = scenarioContext.ScenarioInfo.Tags;

        if (tags == null || tags.Length == 0)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must have at least one @tag");
        }

        string[] distinctTags = tags
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctTags.Length != tags.Length)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' contains duplicate tags");
        }

        List<string> matchedLevelTags = distinctTags
            .Where(tag => Enum.TryParse<TestLevelTag>(tag, true, out _))
            .ToList();

        if (matchedLevelTags.Count == 0)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must contain exactly one test level tag: " +
                string.Join(", ", Enum.GetNames<TestLevelTag>().Select(t => "@" + t.ToLower())));
        }

        if (matchedLevelTags.Count > 1)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must not contain multiple test level tags. Found: " +
                string.Join(", ", matchedLevelTags));
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
            {"public ilocator",LocatorAccessModifiers },
            {"internal ilocator",LocatorAccessModifiers },
            {"Task.Delay", "Avoid Task.Delay in tests. Use explicit waits instead"},
            {"Thread.Sleep", "Do not use Thread.Sleep. Use proper waits instead"}
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

    private static void ValidateAllPagesRegistered()
    {
        VerifyAllTypeWithSuffixAreRegistered(
            assemblyAnchor: typeof(RegisterPageObjects),
            suffix: "Page",
            registerAction: sc => sc.AddPageObjects(),
            friendlyName: "Page",
            registerMethodName: "RegisterPageObjects.AddPageObjects()");
    }

    private static void VerifyAllTypeWithSuffixAreRegistered(
        Type assemblyAnchor,
        string suffix,
        Action<IServiceCollection> registerAction,
        string friendlyName,
        string registerMethodName)
    {
        ServiceCollection services = new();
        registerAction(services);
        Assembly assembly = assemblyAnchor.Assembly;

        bool IsConcreteTarget(Type type)
        {
            return type.IsClass &&
            !type.IsAbstract &&
            !type.IsGenericTypeDefinition &&
            type.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) &&
            !type.Name.StartsWith("Base", StringComparison.Ordinal);
        }

        List<Type> allTypes = assembly.GetTypes().Where(IsConcreteTarget).ToList();

        List<Type> register = services
            .Where(s => s.ImplementationType != null)
            .Select(s => s.ImplementationType!)
            .Where(t => t.Name.EndsWith(suffix, StringComparison.Ordinal))
            .ToList();

        List<Type> unregisterPages = allTypes.Where(t => !register.Contains(t)).ToList();

        if (unregisterPages.Count != 0)
        {
            Assert.Fail(
                $"The following {friendlyName} were not registered in {registerMethodName}:\n" +
                string.Join("\n", unregisterPages.Select(t => "- " + t.FullName)) +
                $"\n\n Action: Add them inside{registerMethodName}");
        }

    }
}
