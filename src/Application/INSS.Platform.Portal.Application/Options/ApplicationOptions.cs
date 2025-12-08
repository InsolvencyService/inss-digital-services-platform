using System.Reflection;

namespace INSS.Platform.Portal.Application.Options;

public sealed class ApplicationOptions
{
    private readonly List<Assembly> _assemblies = [];

    internal ApplicationOptions()
    {
    }
    
    public IEnumerable<Assembly> ModelAssemblies => _assemblies;

    public void AddModelAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }
}