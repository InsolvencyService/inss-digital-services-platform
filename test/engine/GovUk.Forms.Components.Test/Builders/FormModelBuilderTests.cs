using GovUk.Forms.Components.Builders;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Components.Test.Builders;

public class FormModelBuilderTests
{
    [Fact]
    public void FormWithSections_ValidateAndComplete_ReturnsFormWithBothSections()
    {
        FormModel form = CreateForm();
        
        Assert.Equal(2, form.Sections.Count);
        Assert.Equal("/example/your-details", form.Sections[0].Path);
        Assert.Equal("/example/your-employee-details", form.Sections[1].Path);
    }
    
    [Fact]
    public void FirstSectionHasPages_ValidateAndComplete_ReturnsFormWithSectionPages()
    {
        FormModel form = CreateForm();

        SectionModel section = form.Sections["Your Details"];
        Assert.Equal(6, section.Pages.Count);
        Assert.NotNull(section.Pages.GetFirstOf<FullNameModel>());
        Assert.NotNull(section.Pages.GetFirstOf<AddressModel>());
        Assert.NotNull(section.Pages.GetFirstOf<AgeModel>());
        Assert.NotNull(section.Pages.GetFirstOf<SalaryModel>());
        Assert.NotNull(section.Pages.GetFirstOf<BankAccountModel>());
        Assert.NotNull(section.Pages.GetFirstOf<SummaryModel>());
    }
    
    [Fact]
    public void FirstSectionHasPages_ValidateAndComplete_ReturnsFormWithSectionPagePaths()
    {
        FormModel form = CreateForm();

        SectionModel section = form.Sections["Your Details"];
        Assert.Equal(6, section.Pages.Count);
        Assert.Equal("/example/your-details/your-name", section.Pages[0].Path);
        Assert.Equal("/example/your-details/your-address", section.Pages[1].Path);
        Assert.Equal("/example/your-details/your-age", section.Pages[2].Path);
        Assert.Equal("/example/your-details/your-salary", section.Pages[3].Path);
        Assert.Equal("/example/your-details/your-bank-account", section.Pages[4].Path);
        Assert.Equal("/example/your-details/summary", section.Pages[5].Path);
    }
    
    [Fact]
    public void SecondSectionHasPages_ValidateAndComplete_ReturnsFormWithSectionPages()
    {
        FormModel form = CreateForm();
        
        SectionModel section = form.Sections["Employee Details"];
        Assert.Equal(2, section.Pages.Count);
        AddAnotherGroup addAnotherGroup = section.Pages.GetFirstOf<AddAnotherGroup>();
        Assert.NotNull(addAnotherGroup.Pages.GetFirstOf<FullNameModel>());
        Assert.NotNull(addAnotherGroup.Pages.GetFirstOf<AgeModel>());
        Assert.NotNull(addAnotherGroup.Pages.GetFirstOf<CheckAnswersModel>());
        Assert.NotNull(addAnotherGroup.Pages.GetFirstOf<AddAnotherModel>());
        Assert.NotNull(form.Sections[1].Pages.GetFirstOf<SummaryModel>());
    }
    
    [Fact]
    public void SecondSectionHasPages_ValidateAndComplete_ReturnsFormWithSectionPagePaths()
    {
        FormModel form = CreateForm();

        SectionModel section = form.Sections["Employee Details"];
        Assert.Equal(2, section.Pages.Count);
        AddAnotherGroup addAnotherGroup = section.Pages.GetFirstOf<AddAnotherGroup>();
        Assert.Equal("/example/your-employee-details/employee-name", addAnotherGroup.Pages[0].Path);
        Assert.Equal("/example/your-employee-details/employee-age", addAnotherGroup.Pages[1].Path);
        Assert.Equal("/example/your-employee-details/check-employee-details", addAnotherGroup.Pages[2].Path);
        Assert.Equal("/example/your-employee-details/add-another-employee", addAnotherGroup.Pages[3].Path);
        Assert.Equal("/example/your-employee-details/summary", form.Sections[1].Pages[1].Path);
    }

    private static FormModel CreateForm()
    {
        return FormModelBuilder
            .Create("example")
            
            .AddSection("Your Details", "your-details")
            .AddPage<FullNameModel>("Your name", "your-name")
            .AddPage<AddressModel>("Your address", "your-address")
            .AddPage<AgeModel>("Your age", "your-age")
            .AddPage<SalaryModel>("Your salary", "your-salary")
            .AddPage<BankAccountModel>("Your bank account", "your-bank-account")
            .EndSection<SummaryModel>("Your summary", "summary")
            
            .AddSection("Employee Details", "your-employee-details")
            .AddGroup<AddAnotherGroup>("Employees")
            .AddGroupPage<FullNameModel>("Employee name", "employee-name")
            .AddGroupPage<AgeModel>("Employee age", "employee-age")
            .AddGroupPage<CheckAnswersModel>("Check employee details", "check-employee-details")
            .AddFinalGroupPage<AddAnotherModel>("Employee details", "add-another-employee")
            .EndSection<SummaryModel>("Employee summary", "summary")
            
            .ValidateAndComplete();
    }
}