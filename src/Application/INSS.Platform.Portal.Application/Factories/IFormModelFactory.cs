using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Factories;

public interface IFormModelFactory
{
    Task<FormModel> CreateAsync();
}