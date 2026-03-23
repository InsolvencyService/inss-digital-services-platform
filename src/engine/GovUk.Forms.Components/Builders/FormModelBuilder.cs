using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Components.Builders;

public sealed class FormModelBuilder
{
    private readonly FormModel _form;
    
    private FormModelBuilder(string path, SubmitTypes submitType)
    {
        _form = new FormModel { Path = $"/{path}", SubmitType = submitType};
    }
    
    public static FormModelBuilder Create(string path, SubmitTypes submitType = SubmitTypes.Form)
    {
        return new FormModelBuilder(path, submitType);
    }

    public SectionModelBuilder AddSection(string title, string path)
    {
        return new SectionModelBuilder(this, _form, title, path);
    }

    public FormModel ValidateAndComplete()
    {
        _form.Validate();
        return _form;
    }
}