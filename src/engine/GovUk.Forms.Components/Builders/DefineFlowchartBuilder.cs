using GovUk.Forms.Application.Factories;
using GovUk.Forms.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Builders;

public abstract class DefineFlowchartBuilder
{
    public abstract void Construct(IServiceCollection services);
    
    protected static FormModel GetForm(IServiceCollection services)
    {
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IFormFactory formFactory = serviceProvider.GetRequiredService<IFormFactory>();
        return formFactory.Create();
    }
}