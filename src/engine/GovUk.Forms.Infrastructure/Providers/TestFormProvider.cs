using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Serialization;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class TestFormProvider : IFormProvider
{
    public FormModel Create(ContentPath path)
    {
        string rootPath = path.GetRoot().Value;
        string formPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Forms", rootPath.Replace("/", "") + ".json");
        string json = File.ReadAllText(formPath);
        return FormSerializer.DeserializeForm(json);
    }
}