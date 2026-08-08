using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Validators;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Loading;
using Inss.Platform.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Extensions;

public static class PageBuildExtensions
{
    extension(PageModel page)
    {
        public void Register(IServiceCollection services)
        {
            page.RegisterPageValidator(services);
            page.RegisterNextPageNavigator(services);
            page.RegisterLoaders(services);
            page.RegisterValidations(services);
        }

        private void RegisterPageValidator(IServiceCollection services)
        {
            if (page.PageValidator is not null)
            {
                services.AddKeyedSingleton(typeof(IPageValidator), page.Path.Value, page.PageValidator);
            }
        }
        
        private void RegisterNextPageNavigator(IServiceCollection services)
        {
            if (page.NextPageNavigator is not null)
            {
                services.AddKeyedSingleton(typeof(INextPageNavigator), page.Path.Value, page.NextPageNavigator);
            }
        }
        
        private void RegisterLoaders(IServiceCollection services)
        {
            foreach (ComponentModel component in page.Components)
            {
                foreach (Loader loader in component.Loaders)
                {
                    services.AddKeyedSingleton(typeof(IComponentLoader), component.Id.Value, loader.LoaderType);
                }
            }
        }
        
        private void RegisterValidations(IServiceCollection services)
        {
            foreach (ComponentModel component in page.Components)
            {
                foreach (ValidationRule validation in component.Validations)
                {
                    services.AddKeyedSingleton(
                        typeof(IComponentValidator), 
                        component.Id.Value, 
                        (provider, _) => ActivatorUtilities.CreateInstance(provider, validation.ValidatorType, validation));
                }
            }
        }
    }
}