using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;

namespace Inss.Platform.Application.Validators;

public sealed class ComponentContext
{
    public ComponentContext(PageModel page, ComponentModel component)
    {
        Page = page;
        Component = component;
    }
    
    public PageModel Page { get; }
    
    public ComponentModel Component { get; }
}