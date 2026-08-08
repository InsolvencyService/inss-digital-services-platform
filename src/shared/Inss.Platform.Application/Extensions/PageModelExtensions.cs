using System.ComponentModel.DataAnnotations;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Validators;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Application.Extensions;

public static class PageModelExtensions
{
    extension(PageModel page)
    {
        public async ValueTask LoadAsync(AppModel app, QueryParamList queryParams, IServiceProvider serviceProvider)
        {
            foreach (ComponentModel component in page.Components)
            {
                IEnumerable<IComponentLoader> loaders = serviceProvider.GetKeyedServices<IComponentLoader>(component.Id.Value);
            
                foreach (IComponentLoader loader in loaders)
                {
                    LoaderContext context = new(app, page, component, queryParams);
                    await loader.LoadAsync(context);
                }
            }
        }

        public void CopyComponentValuesTo(PageModel currentPage)
        {
            foreach (ComponentModel currentComponent in currentPage.Components.Where(c => c.ComponentType == ComponentTypes.Bindable))
            {
                ComponentModel component = page.Components.GetComponent(currentComponent.Id);
                component.CopyTo(currentComponent);
            }
        }
        
        public async ValueTask ValidateAsync(IServiceProvider serviceProvider)
        {
            List<PageValidationError> pageValidationErrorList = [];
        
            foreach (ComponentModel component in page.Components.Where(c => c.ComponentType == ComponentTypes.Bindable))
            {
                IEnumerable<IComponentValidator> validators = serviceProvider.GetKeyedServices<IComponentValidator>(component.Id.Value);
            
                foreach (IComponentValidator validator in validators)
                {
                    ComponentContext context = new(page, component);
                    ValidationResult[] componentValidations = await validator.ValidateAsync(context);
                
                    pageValidationErrorList.AddRange(componentValidations.Select(vr => new PageValidationError
                    {
                        Properties = vr.MemberNames.ToArray(),
                        Message = vr.ErrorMessage ?? string.Empty
                    }));
                }
            }
        
            if (pageValidationErrorList.Count > 0)
            {
                page.PageValidationInfo = new PageValidationInfo { Errors = pageValidationErrorList.ToArray() };
            }
            else if (page.PageValidator is not null)
            {
                IPageValidator? pageValidator = serviceProvider.GetKeyedService<IPageValidator>(page.Path.Value);
                
                if (pageValidator is not null)
                {
                    PageContext context = new(page);
                    await pageValidator.ValidateAsync(context);
                }
            }
        }
    }
}