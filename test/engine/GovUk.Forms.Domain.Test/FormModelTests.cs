using GovUk.Forms.Domain.Exceptions;
using Xunit;

namespace GovUk.Forms.Domain.Test;

public class FormModelTests
{
    [Fact]
    public void PathToUnknownModel_GetContent_ThrowsException()
    {
        const string unknownPath = "/form/your-details/unknown";
        FormModel form = TestFormModels.CreateWithYourDetailsSection();

        ModelException exception = Assert.Throws<ModelException>(() => form.GetContent(unknownPath));

        Assert.Equal($"Unable to find page for path {unknownPath}.", exception.Message);
    }
    
    [Theory]
    [InlineData("/form/your-details/your-address")]
    [InlineData("/form/your-details/your-address/")]
    public void PathToAddress_GetContent_ReturnsAddressModel(string path)
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();

        ContentModel content = form.GetContent(path);

        Assert.NotNull(content);
        Assert.IsType<AddressModel>(content);
    }
    
    [Fact]
    public void PathToUnknownModel_GetSectionForPage_ThrowsException()
    {
        const string unknownPath = "/form/your-details/unknown";
        FormModel form = TestFormModels.CreateWithYourDetailsSection();

        ModelException exception = Assert.Throws<ModelException>(() => form.GetSectionForPage(unknownPath));

        Assert.Equal($"Unable to find section for page path {unknownPath}.", exception.Message);
    }
    
    [Fact]
    public void PathToModel_GetSectionForPage_ReturnsSection()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();

        SectionModel section = form.GetSectionForPage("/form/your-details/your-address");

        Assert.NotNull(section);
    }

    [Fact]
    public void SectionWithPages_GetAllPages_ReturnsAllPages()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();

        PageModelList pages = form.GetAllPages();

        Assert.Equal(6, pages.Count);
        Assert.NotNull(pages.FirstOrDefault(p => p is FullNameModel));
        Assert.NotNull(pages.FirstOrDefault(p => p is AddressModel));
        Assert.NotNull(pages.FirstOrDefault(p => p is AgeModel));
        Assert.NotNull(pages.FirstOrDefault(p => p is SalaryModel));
        Assert.NotNull(pages.FirstOrDefault(p => p is BankAccountModel));
        Assert.NotNull(pages.FirstOrDefault(p => p is SummaryModel));
    }
    
    [Fact]
    public void FormWithNoSections_Validate_ThrowsException()
    {
        FormModel form = new();

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The form contains no sections. At least 1 section should exist.", exception.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("example")]
    [InlineData("example/")]
    public void FormWithInvalidPath_Validate_ThrowsException(string path)
    {
        FormModel form = new() { Path = path };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The form path is an invalid format.", exception.Message);
    }

    [Fact]
    public void SectionWithNoPages_Validate_ThrowsException()
    {
        FormModel form = new() { Path = "/example", Sections = [new SectionModel { Path = "/example/section1", Title = "Section1" }] };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The section contains no pages. At least 1 page should exist.", exception.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("example")]
    [InlineData("example/")]
    public void SectionWithInvalidPath_Validate_ThrowsException(string path)
    {
        FormModel form = new() { Path = "/example", Sections = [new SectionModel { Path = path, Title = "Section1" }] };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The section path is an invalid format.", exception.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SectionWithMissingTitle_Validate_ThrowsException(string title)
    {
        FormModel form = new() { Path = "/example", Sections = [new SectionModel { Path = "/example/section1", Title = title }] };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The section title is missing.", exception.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("example")]
    [InlineData("example/")]
    public void PageWithInvalidPath_Validate_ThrowsException(string path)
    {
        FormModel form = new()
        {
            Path = "/example",
            Sections =
            [
                new SectionModel
                {
                    Path = "/example/section1", 
                    Title = "Section1",
                    Pages = [new FullNameModel { Title = "Fullname", Path = path }]
                }
            ]
        };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The page path is an invalid format.", exception.Message);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void PageWithMissingTitle_Validate_ThrowsException(string title)
    {
        FormModel form = new()
        {
            Path = "/example",
            Sections =
            [
                new SectionModel
                {
                    Path = "/example/section1", 
                    Title = "Section1",
                    Pages = [new FullNameModel { Title = title, Path = "/example/section1/fullname" }]
                }
            ]
        };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("The page title is missing.", exception.Message);
    }
    
    [Fact]
    public void AddAnotherWithNoWorkingPages_Validate_ThrowsException()
    {
        FormModel form = new()
        {
            Path = "/example",
            Sections =
            [
                new SectionModel
                {
                    Path = "/example/section1", 
                    Title = "Section1",
                    Pages = [
                        new AddAnotherGroup
                        {
                            Pages = [new AddAnotherModel { Title = "Add Another", Path = "/example/section1/add-another" }]
                        }
                    ]
                }
            ]
        };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("There are no working pages for the add another group defined.", exception.Message);
    }

    [Fact]
    public void SectionGroupPageMissingGroup_Validate_ThrowsException()
    {
        FormModel form = new()
        {
            Path = "/example",
            Sections =
            [
                new SectionModel
                {
                    Path = "/example/section1", 
                    Title = "Section1", 
                    Pages = [new AddAnotherGroup()]
                }
            ]
        };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("Missing group Id defined in section /example/section1.", exception.Message);
    }
    
    [Fact]
    public void SectionHasDuplicateGroupIds_Validate_ThrowsException()
    {
        FormModel form = new()
        {
            Path = "/example",
            Sections =
            [
                new SectionModel
                {
                    Path = "/example/section1", 
                    Title = "Section1", 
                    Pages = [
                        new AddAnotherGroup{ MetaData = {Group = "Group1"}}, 
                        new AddAnotherGroup{ MetaData = {Group = "Group1"}}
                    ]
                }
            ]
        };

        FormValidationException exception = Assert.Throws<FormValidationException>(() => form.Validate());

        Assert.Contains("Group Id Group1 already defined in section /example/section1.", exception.Message);
    }
    
    [Fact]
    public void ValidForm_Validate_DoesNotThrowException()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        try
        {
            form.Validate();
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    [Fact]
    public void CompletedForm_GetSubmittable_ReturnsFormDetails()
    {
        const string sectionTitle = "Your Details";
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        TestSectionDefaults.YourDetails(form.Sections[sectionTitle]);
        
        FormModel submittableForm = form.GetSubmittable();
        
        Assert.Equal(form.Id, submittableForm.Id);
        Assert.Equal(form.Path, submittableForm.Path);
        Assert.Equal(form.Sections.Count, submittableForm.Sections.Count);
    }
    
    [Fact]
    public void CompletedForm_GetSubmittable_ReturnsFormSection()
    {
        const string sectionTitle = "Your Details";
        const int pageCountLessSummary = 1;
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        TestSectionDefaults.YourDetails(form.Sections[sectionTitle]);
        
        FormModel submittableForm = form.GetSubmittable();

        SectionModel section = form.Sections[sectionTitle];
        SectionModel submittableSection = submittableForm.Sections[sectionTitle];
        Assert.Equal(section.Pages.Count - pageCountLessSummary, submittableSection.Pages.Count);
    }
    
    [Fact]
    public void CompletedForm_GetSubmittable_ReturnsFormSectionPages()
    {
        const string sectionTitle = "Your Details";
        const int pageCountLessSummary = 1;
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        TestSectionDefaults.YourDetails(form.Sections[sectionTitle]);
        
        FormModel submittableForm = form.GetSubmittable();

        SectionModel section = form.Sections[sectionTitle];
        SectionModel submittableSection = submittableForm.Sections[sectionTitle];
        Assert.Equal(section.Pages.Count - pageCountLessSummary, submittableSection.Pages.Count);
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        Assert.NotNull(submittableSection.Pages.FirstOrDefault(p => p.Id == fullName.Id && p.Path == fullName.Path));
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        Assert.NotNull(submittableSection.Pages.FirstOrDefault(p => p.Id == address.Id && p.Path == address.Path));
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        Assert.NotNull(submittableSection.Pages.FirstOrDefault(p => p.Id == age.Id && p.Path == age.Path));
        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        Assert.NotNull(submittableSection.Pages.FirstOrDefault(p => p.Id == salary.Id && p.Path == salary.Path));
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        Assert.NotNull(submittableSection.Pages.FirstOrDefault(p => p.Id == bankAccount.Id && p.Path == bankAccount.Path));
    }
    
    [Fact]
    public void CompletedForm_GetSubmittable_DoesNotIncludeReturnsFormSectionSummary()
    {
        const string sectionTitle = "Your Details";
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        TestSectionDefaults.YourDetails(form.Sections[sectionTitle]);
        
        FormModel submittableForm = form.GetSubmittable();
        
        SectionModel submittableSection = submittableForm.Sections[sectionTitle];
        Assert.Null(submittableSection.Pages.FirstOrDefault(p => p is SummaryModel));
    }
}