using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Application.PageFlow;

public class DefaultPageValidator : IPageValidator
{
    public static readonly IPageValidator Default = new DefaultPageValidator();
    private const bool AllProperties = true;
    
    public virtual ValueTask ValidateAsync(ValidatePageContext context)
    {
        ValidationContext validationContext = new(context.CurrentPage);
        Validator.TryValidateObject(context.CurrentPage, validationContext, context.ValidationResults, AllProperties);
        return ValueTask.CompletedTask;
    }
}