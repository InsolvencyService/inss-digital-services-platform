using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Components.Builders;

public abstract class DefineFlowchartBuilder
{
    public abstract void Construct(IServiceCollection services);
    
    protected static FormModel GetForm(IServiceCollection services, ContentPath rootPath)
    {
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        IFormProvider formProvider = serviceProvider.GetRequiredService<IFormProvider>();
        return formProvider.Create(rootPath);
    }
}