using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Validation;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Extensions;

public static class PageBuildExtensions
{
    extension(PageModel page)
    {
        public PageModel Register(IServiceCollection services)
        {
            page.RegisterNextPageNavigator(services);
            page.RegisterLoaders(services);
            page.RegisterValidations(services);
            return page;
        }

        private void RegisterNextPageNavigator(IServiceCollection services)
        {
            if (page.NextPageNavigator is not null)
            {
                services.AddKeyedSingleton(typeof(INextPageNavigator), page.Path.Value, page.NextPageNavigator);
            }
        }
        
        private Inss.Platform.Domain.PageModel RegisterLoaders(IServiceCollection services)
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
        
        private Inss.Platform.Domain.PageModel RegisterValidations(IServiceCollection services)
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