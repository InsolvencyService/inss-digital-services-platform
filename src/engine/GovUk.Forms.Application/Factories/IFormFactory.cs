using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Factories;

public interface IFormFactory
{
    FormModel Create();
}

public sealed class FormFactory : IFormFactory
{
    private readonly FormModel _templateForm;

    public FormFactory(FormModel templateForm)
    {
        _templateForm = templateForm;
    }
    
    public FormModel Create()
    {
        FormModel form = new() { Path = _templateForm.Path, SubmitType = _templateForm.SubmitType };

        foreach (SectionModel section in _templateForm.Sections)
        {
            form.Sections.Add(new SectionModel{ Title = section.Title, Path = section.Path, SubmitType = section.SubmitType });
        }

        return form;
    }
}