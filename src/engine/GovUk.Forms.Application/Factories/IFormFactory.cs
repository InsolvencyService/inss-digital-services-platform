using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Factories;

public interface IFormFactory
{
    FormModel Create();
}