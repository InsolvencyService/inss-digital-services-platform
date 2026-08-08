namespace Inss.Platform.Application.Validation;

public interface IPageValidator
{
    ValueTask ValidateAsync(PageContext context);
}