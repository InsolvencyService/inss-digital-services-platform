using GovUk.Forms.Domain.Exceptions;
using Xunit;

namespace GovUk.Forms.Domain.Test;

public class SectionModelListTests
{
    [Fact]
    public void PathNotMatchExistingSection_Index_ThrowsException()
    {
        SectionModel section1 = new() { Path = "/example/section1" };
        SectionModel section2 = new() { Path = "/example/section2" };
        FormModel form = new() { Sections = [section1] };

        ModelException exception = Assert.Throws<ModelException>(() => form.Sections[section2.Path]);
        
        Assert.Equal($"Unable to find the section using the path /example/section2.", exception.Message);
    }
    
    [Fact]
    public void PathMatchesExistingSection_Index_ReturnsSection()
    {
        SectionModel section1 = new() { Path = "/example/section1" };
        SectionModel section2 = new() { Path = "/example/section2" };
        FormModel form = new() { Sections = [section1, section2] };

        SectionModel foundSection = form.Sections[section1.Path];
        
        Assert.Equal(section1, foundSection);
    }
    
    [Fact]
    public void TitleNotMatchExistingSection_Index_ThrowsException()
    {
        SectionModel section1 = new() { Title = "Section1" };
        SectionModel section2 = new() { Title = "Section2" };
        FormModel form = new() { Sections = [section1] };

        ModelException exception = Assert.Throws<ModelException>(() => form.Sections[section2.Title]);
        
        Assert.Equal($"Unable to find the section using the title Section2.", exception.Message);
    }
    
    [Fact]
    public void TitleMatchesExistingSection_Index_ReturnsSection()
    {
        SectionModel section1 = new() { Title = "Section1" };
        SectionModel section2 = new() { Title = "Section2" };
        FormModel form = new() { Sections = [section1, section2] };

        SectionModel foundSection = form.Sections[section1.Title];
        
        Assert.Equal(section1, foundSection);
    }
    
    [Fact]
    public void SectionNotStarted_SetInProgress_SetsSectionStarted()
    {
        SectionModel section = new() { Title = "Section1", StartedDate = null };

        section.SetInProgress();
        
        Assert.NotNull(section.StartedDate);
    }
}