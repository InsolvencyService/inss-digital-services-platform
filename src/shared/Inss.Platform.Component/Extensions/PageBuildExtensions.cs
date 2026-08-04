using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Validation;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Extensions;

public static class PageBuildExtensions
{
    extension(Inss.Platform.Domain.Page page)
    {
        public Inss.Platform.Domain.Page Register(IServiceCollection services)
        {
            page.RegisterNextPageNavigator(services);
            page.RegisterLoaders(services);
            page.RegisterValidations(services);
            return page;
        }

        private Inss.Platform.Domain.Page RegisterNextPageNavigator(IServiceCollection services)
        {
            if (page.NextPageNavigator is not null)
            {
                services.AddKeyedSingleton(typeof(INextPageNavigator), page.Path.Value, page.NextPageNavigator);
            }

            return page;
        }
        
        private Inss.Platform.Domain.Page RegisterLoaders(IServiceCollection services)
        {
            foreach (Inss.Platform.Domain.Components.Component component in page.Components)
            {
                foreach (Loader loader in component.Loaders)
                {
                    services.AddKeyedSingleton(typeof(IComponentLoader), component.Id.Value, loader.LoaderType);
                }
            }

            return page;
        }
        
        private Inss.Platform.Domain.Page RegisterValidations(IServiceCollection services)
        {
            foreach (Inss.Platform.Domain.Components.Component component in page.Components)
            {
                foreach (ValidationRule validation in component.Validations)
                {
                    services.AddKeyedSingleton(
                        typeof(IComponentValidator), 
                        component.Id.Value, 
                        (provider, _) => ActivatorUtilities.CreateInstance(provider, validation.ValidatorType, validation));
                }
            }

            return page;
        }
    }
}