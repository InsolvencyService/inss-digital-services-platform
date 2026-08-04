using Inss.Platform.Domain.Components;

namespace Inss.Platform.Application.Loaders;

public interface IComponentLoader
{
    ValueTask LoadAsync(ComponentModel component);
}