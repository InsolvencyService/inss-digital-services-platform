namespace Inss.Platform.Application.Validators;

public interface IPageValidator
{
    ValueTask ValidateAsync(PageContext context);
}