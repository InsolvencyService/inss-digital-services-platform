using System.Reflection;

namespace GovUk.Forms.Domain.Serialization;

public static class ModelAssemblies
{
    public static Assembly[] GetAll()
    {
        List<Assembly> modelAssemblies = [typeof(PageModel).Assembly];
                
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a =>
                     a.FullName?.StartsWith("Demo.", StringComparison.OrdinalIgnoreCase) == true ||
                     a.FullName?.StartsWith("Inss.", StringComparison.OrdinalIgnoreCase) == true))
        {
            modelAssemblies.Add(assembly);
        }
        
        return  modelAssemblies.ToArray();
    }
}