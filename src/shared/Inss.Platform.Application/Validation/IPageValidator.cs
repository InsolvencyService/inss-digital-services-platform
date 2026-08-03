using Inss.Platform.Domain;

namespace Inss.Platform.Application.Validation;

public interface IPageValidator
{
    ValueTask ValidateAsync(Page page);
}