using Inss.Platform.Domain.Components.Searching;

namespace Inss.Platform.Application.Providers;

public interface ISearchConfigProvider
{
    SearchDefinition LoadConfig();
}