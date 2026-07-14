using GovUk.Forms.Domain;

namespace GovUk.Forms.Components.Builders;

public sealed class GroupModelBuilder
{
    private readonly SectionModelBuilder _sectionModelBuilder;
    private readonly GroupPageModel _group;
    private readonly SectionModel _section;

    internal GroupModelBuilder(GroupPageModel group, SectionModel section, SectionModelBuilder sectionModelBuilder)
    {
        _group = group;
        _section = section;
        _sectionModelBuilder = sectionModelBuilder;
    }
    
    public GroupModelBuilder AddGroupPage<TPage>(
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
                Group = _group.MetaData.Group,
                SubmitButtonText = submitButtonText
            }
        };
        _group.Pages.Add(page);
        return this;
    }
    
    public SectionModelBuilder AddFinalGroupPage<TPage>(
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
                Group = _group.MetaData.Group,
                SubmitButtonText = submitButtonText
            }
        };
        _group.Pages.Add(page);
        
        _section.Pages.Add(_group);
        
        return _sectionModelBuilder;
    }
}