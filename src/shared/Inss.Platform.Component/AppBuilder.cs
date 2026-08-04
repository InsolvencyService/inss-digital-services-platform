using Inss.Platform.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component;

public abstract class AppBuilder
{
    public abstract PagePath[] Build(IServiceCollection services);
}