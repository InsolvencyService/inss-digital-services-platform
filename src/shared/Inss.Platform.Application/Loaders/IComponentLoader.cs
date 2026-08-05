namespace Inss.Platform.Application.Loaders;

public interface IComponentLoader
{
    ValueTask LoadAsync(LoaderContext context);
}