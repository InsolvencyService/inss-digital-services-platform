using Inss.Platform.Domain.Components.Searching.Support;

namespace Inss.Platform.Application.Providers;

public interface ISearchConfigProvider
{
    SearchDefinition LoadConfig();
}