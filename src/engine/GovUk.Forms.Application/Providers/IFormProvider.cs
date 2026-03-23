using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.Providers;

public interface IFormProvider
{
    FormModel Create(ContentPath path);
}