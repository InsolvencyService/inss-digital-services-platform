using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Components.Builders;

public sealed class SectionModelBuilder
{
    private readonly FormModelBuilder _formModelBuilder;
    private readonly FormModel _form;
    private readonly SectionModel _section;
    
    internal SectionModelBuilder(FormModelBuilder formModelBuilder, FormModel form, string title, string path)
    {
        _formModelBuilder = formModelBuilder;
        _form = form;
        _section = new SectionModel { Title = title, Path = $"{form.Path}/{path}", SubmitType = _form.SubmitType };
    }

    public SectionModelBuilder AddPage<TPage>(
        string title, 
        string path, 
        string? question = null, 
        string? hint = null,
        string? description = null,
        string? submitButtonText = null) 
        where TPage : PageModel, new()
    {
        TPage page = new()
        {
            Title = title, 
            Path = $"{_section.Path}/{path}", 
            SubmitType = _section.SubmitType,
            MetaData =
            {
                Question = question,
                Hint = hint,
                Description = description,
                SubmitButtonText = submitButtonText 
            }
        };
        _section.Pages.Add(page);
        return this;
    }

    public SearchModelBuilder AddSearchPage<TPage>(
        string title, 
        string path, 
        string? question = null, 
        string? hint = null,
        string? description = null,
        string? submitButtonText = null) 
        where TPage : SearchModel, new()
    {
        TPage page = new()
        {
            Title = title, 
            Path = $"{_section.Path}/{path}", 
            SubmitType = _section.SubmitType,
            MetaData =
            {
                Question = question,
                Hint = hint,
                Description = description,
                SubmitButtonText = submitButtonText 
            }
        };
        _section.Pages.Add(page);
        return new SearchModelBuilder(this, page);
    }
    
    public GroupModelBuilder AddGroup<TGroup>(GroupId group) where TGroup : GroupPageModel, new()
    {
        return new GroupModelBuilder(new TGroup { MetaData = { Group = group } }, _section, this);
    }
    
    public FormModelBuilder EndSection<TPage>(
        string title, 
        string path,
        string? question = null, 
        string? hint = null,
        string? description = null,
        string? submitButtonText = null) 
        where TPage : PageModel, new()
    {
        TPage page = new()
        {
            Title = title, 
            Path = $"{_section.Path}/{path}", 
            SubmitType = _section.SubmitType,
            MetaData =
            {
                Question = question,
                Hint = hint,
                Description = description,
                SubmitButtonText = submitButtonText 
            }
        };
        _section.Pages.Add(page);
        _form.Sections.Add(_section);
        return _formModelBuilder;
    }
}