using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Pages;
using GovUk.Forms.HostApp.UI.Test.Tags;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public static class TestValidator
{
    private static bool _bindingsValidated;
    private static bool _codeValidated;
    private const string DontUseXpath = "Do not use XPath in loactors. Follow Playwright best practices.\n\n";
    private const string LocatorAccessModifiers = "Locators must be private or protected\n\n";
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
        VerifyAllCoordinatorsRegistered();
    }


    private static void ValidateScenarioTag(ScenarioContext scenarioContext)
    {
        string[] tags = scenarioContext.ScenarioInfo.Tags;

        if (tags == null || tags.Length == 0)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must have at least one tag.");
        }

        string[] distinctTags = tags
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (distinctTags.Length != tags.Length)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' contains duplicate tags.");
        }

        List<string> matchedTestLevelTags = distinctTags
            .Where(tag =>
                Enum.GetNames<TestLevelTag>()
                    .Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (matchedTestLevelTags.Count == 0)
        {
            throw new ArgumentException(
                $"Scenario '{scenarioContext.ScenarioInfo.Title}' must contain at least one test level tag: " +
                string.Join(", ",
                    Enum.GetNames<TestLevelTag>().Select(t => "@" + t.ToLower())));
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
            {"//table",DontUseXpath },
            {"//*[",DontUseXpath },
            { "//div",DontUseXpath },
            {"/div/",DontUseXpath },
            {"//td",DontUseXpath },
            {"//li",DontUseXpath },
            {"/ul/li",DontUseXpath },
            {"//tr",DontUseXpath },
            {"//span",DontUseXpath },
            {"//button",DontUseXpath },
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
            {"Task.Delay", "Avoid Task.Delay in tests. Use explicit waits instead\n"},
            {"Thread.Sleep", "Do not use Thread.Sleep. Use Playwright waits instead\n"}
        };

        Regex regex = new(
            string.Join("|", invalidKeywords.Keys.Select(Regex.Escape)),
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        HashSet<string> ignores = new(StringComparer.OrdinalIgnoreCase)
        {
            "bin", "obj", ".git", ".vs"
        };

        foreach (string file in rootDirectory!
                     .FindAllFilePaths(".cs", ignores)
                     .Where(f => !string.Equals(f, currentFilePath, StringComparison.OrdinalIgnoreCase)))
        {
            int lineNum = 0;

            foreach (string line in File.ReadLines(file))
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

    private static void VerifyAllCoordinatorsRegistered()
    {
        VerifyAllTypeWithSuffixAreRegistered(
            assemblyAnchor: typeof(RegisterCoordinators),
            suffix: "Coordinator",
            registerAction: sc => sc.AddCoordinators(),
            friendlyName: "Coordinator",
            registerMethodName: "RegisterCoordinators.AddCoordinators()");
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
                $"These {friendlyName} classes exist but are not registered in {registerMethodName}:\n\n" +
                string.Join("\n", unregisterPages.Select(t => t.FullName)) +
                $"\n\n Action: Add them inside {registerMethodName}. \n\n");
        }

    }

    public static void VerifyStepDefinitionsUseOnlyCoordinators(Assembly assembly)
    {
        Type[] allowedStepDependencies =
        [
            typeof(ScenarioContext),
             typeof(FeatureContext),
             typeof(IReqnrollOutputHelper)
        ];

        IEnumerable<Type> stepTypes = assembly.GetTypes()
            .Where(t =>
                t.IsClass &&
                t.GetMethods().Any(m =>
                    m.GetCustomAttributes(inherit: true).Any(a =>
                        a.GetType().Name is "GivenAttribute" or "WhenAttribute" or "ThenAttribute")));

        foreach (Type stepType in stepTypes)
        {
            if (HasDependencyOnBasePage(stepType))
            {
                throw new InvalidOperationException(
                    $"Step definition '{stepType.FullName}' must not depend on BasePage.");
            }

            ConstructorInfo[] constructors = stepType.GetConstructors(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            IEnumerable<ParameterInfo> parameters = constructors
                .SelectMany(c => c.GetParameters())
                .DistinctBy(p => p.ParameterType);

            foreach (ParameterInfo param in parameters)
            {
                bool isCoordinator =
                    param.ParameterType.Name.EndsWith("Coordinator", StringComparison.Ordinal);

                bool isAllowedContext =
                    allowedStepDependencies.Contains(param.ParameterType);

                if (isCoordinator || isAllowedContext)
                {
                    continue;
                }

                string message = string.Join(
                    Environment.NewLine,
                    $"Step definition '{stepType.FullName}' has invalid dependency '{param.ParameterType.FullName}'.",
                    string.Empty,
                    "Steps may only depend on Coordinators, ScenarioContext, or FeatureContext.",
                    string.Empty,
                    "Please move page/business flow logic into a Coordinator.");

                throw new InvalidOperationException(message);
            }
        }
    }
    private static bool HasDependencyOnBasePage(Type stepType)
    {
        Type forbidden = typeof(BasePage);

        const BindingFlags flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // Fields
        bool hasFieldDependency = stepType
            .GetFields(flags)
            .Any(f => forbidden.IsAssignableFrom(f.FieldType));

        // Properties
        bool hasPropertyDependency = stepType
            .GetProperties(flags)
            .Any(p => forbidden.IsAssignableFrom(p.PropertyType));

        // Constructors 
        bool hasCtorDependency = stepType
            .GetConstructors(flags)
            .SelectMany(c => c.GetParameters())
            .Any(p => forbidden.IsAssignableFrom(p.ParameterType));

        return hasFieldDependency || hasPropertyDependency || hasCtorDependency;
    }


}
