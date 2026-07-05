namespace GovUk.Forms.Application.PageFlow;

public interface IPageValidator
{
    ValueTask ValidateAsync(ValidatePageContext context);
}