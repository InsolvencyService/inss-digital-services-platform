using GovUk.Forms.Domain.Exceptions;
using Xunit;

namespace GovUk.Forms.Domain.Test;

public class PageModelListTests
{
    [Fact]
    public void UnknownPath_FindPage_ReturnsNull()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();

        PageModel? page = section.Pages.FindPage("/unknown");
        
        Assert.Null(page);
    }
    
    [Fact]
    public void KnownPath_FindPage_ReturnsPage()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();

        PageModel? page = section.Pages.FindPage("/form/your-details/your-address");
        
        Assert.NotNull(page);
        Assert.IsType<AddressModel>(page);
    }

    [Fact]
    public void UnknownPath_GetPage_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();

        ModelException exception = Assert.Throws<ModelException>(() => section.Pages.GetPage("/unknown"));
        
        Assert.Equal("Unable to find page for path /unknown.", exception.Message);
    }
    
    [Fact]
    public void KnownPath_GetPage_ReturnsPage()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();

        PageModel page = section.Pages.GetPage("/form/your-details/your-address");
        
        Assert.IsType<AddressModel>(page);
    }
    
    [Fact]
    public void ResettingFromAge_ResetDownstream_DoesNotResetFullName()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        
        section.Pages.ResetDownstream(age);

        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        Assert.NotEmpty(fullName.Value);
    }

    [Fact]
    public void ResettingFromAge_ResetDownstream_DoesNotResetAddress()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        
        section.Pages.ResetDownstream(age);

        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        Assert.NotEmpty(address.AddressLine1);
        Assert.NotNull(address.AddressLine2);
        Assert.NotEmpty(address.AddressLine2);
        Assert.NotEmpty(address.TownCity);
        Assert.NotNull(address.County);
        Assert.NotEmpty(address.County);
        Assert.NotEmpty(address.Postcode);
    }
    
    [Fact]
    public void ResettingFromAge_ResetDownstream_DoesNotResetAge()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        
        section.Pages.ResetDownstream(age);
        
        Assert.True(age.Value > 0);
    }
    
    [Fact]
    public void ResettingFromAge_ResetDownstream_ResetsSalary()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        
        section.Pages.ResetDownstream(age);

        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();
        Assert.Equal(0, salary.Value);
    }
    
    [Fact]
    public void ResettingFromAge_ResetDownstream_ResetsBankAccount()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        
        section.Pages.ResetDownstream(age);

        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        Assert.Empty(bankAccount.SortCode);
        Assert.Empty(bankAccount.AccountNumber);
    }
    
    [Fact]
    public void SomeSavedPages_GetCompletedPages_ReturnsCompletedOnly()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        BankAccountModel bankAccount = section.Pages.GetFirstOf<BankAccountModel>();
        bankAccount.CompletedDate = null;
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.SetCompleted();
        
        PageModelList savedPages = section.Pages.GetCompletedPages();

        Assert.NotNull(savedPages.FirstOrDefault(sp => sp is FullNameModel));
        Assert.NotNull(savedPages.FirstOrDefault(sp => sp is AddressModel));
        Assert.NotNull(savedPages.FirstOrDefault(sp => sp is AgeModel));
        Assert.NotNull(savedPages.FirstOrDefault(sp => sp is SalaryModel));
        Assert.Null(savedPages.FirstOrDefault(sp => sp is BankAccountModel));
    }

    [Fact]
    public void AddAnotherGroupedPages_GetFirstOf_ReturnsAssociatedPages()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();

        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        
        Assert.NotNull(groupInfo.WorkingPages.FirstOrDefault(sp => sp is FullNameModel));
        Assert.NotNull(groupInfo.WorkingPages.FirstOrDefault(sp => sp is AgeModel));
        Assert.NotNull(groupInfo.CheckAnswers);
        Assert.NotNull(groupInfo.AddAnother);
        Assert.Null(groupInfo.WorkingPages.FirstOrDefault(sp => sp is SummaryModel));
    }

    [Fact]
    public void UnknownPage_GetFirstOf_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        
        ModelException exception = Assert.Throws<ModelException>(() => section.Pages.GetFirstOf<CheckAnswersModel>());
        
        Assert.Equal($"Unable to find page of type {typeof(CheckAnswersModel)}.", exception.Message);
    }
    
    [Fact]
    public void KnownPage_GetFirstOf_ReturnsPage()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();

        AgeModel age = section.Pages.GetFirstOf<AgeModel>();

        Assert.NotNull(age);
    }

    [Fact]
    public void NonMatchingIdListForAddAnotherItems_RemoveMatchingPages_DoesNotRemoveItems()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        age.Value = 40;
        AddAnotherModel addAnother = groupInfo.AddAnother;
        addAnother.Items.Add(fullName.Clone());
        addAnother.Items.Add(age.Clone());
        
        addAnother.Items.RemoveMatchingPages(["E3752B8F-BEB5-4467-9274-E0B70FC22CEA", "3903FF66-448F-49A4-9F21-A117D4680748"]);
        
        Assert.Equal(2, addAnother.Items.Count);
    }
    
    [Fact]
    public void IdListForAddAnotherItems_RemoveMatchingPages_RemovesItems()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        age.Value = 40;
        AddAnotherModel addAnother = groupInfo.AddAnother;
        addAnother.Items.Add(fullName.Clone());
        addAnother.Items.Add(age.Clone());
        fullName = addAnother.Items.GetFirstOf<FullNameModel>();
        age = addAnother.Items.GetFirstOf<AgeModel>();
        
        addAnother.Items.RemoveMatchingPages([fullName.Id, age.Id]);
        
        Assert.Empty(addAnother.Items);
    }

    [Fact]
    public void UnknownGroup_GetGroup_ThrowsException()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();

        ModelException exception = Assert.Throws<ModelException>(() => section.Pages.GetGroup<AddAnotherGroup>("GroupX"));

        Assert.Equal("Unable to find group page of type GovUk.Forms.Domain.AddAnotherGroup.", exception.Message);
    }
    
    [Fact]
    public void KnownGroup_GetGroup_ReturnsGroupPage()
    {
        SectionModel section = TestSectionModels.CreateSectionWithAddAnother();

        AddAnotherGroup addAnotherGroup = section.Pages.GetGroup<AddAnotherGroup>("Group1");

        Assert.NotNull(addAnotherGroup);
    }
    
    [Fact]
    public void InvalidIndex_GetAtIndex_ThrowsException()
    {
        SectionModel assets = TestSectionModels.CreateAssetSection();

        ModelException exception = Assert.Throws<ModelException>(() => assets.Pages.GetAtIndex<MoneyModel>(4));
        
        Assert.Equal("Unable to find page of type GovUk.Forms.Domain.MoneyModel at index 4.", exception.Message);
    }
    
    [Fact]
    public void MultipleSameModels_GetAtIndex_ReturnsCorrectInstance()
    {
        SectionModel assets = TestSectionModels.CreateAssetSection();

        MoneyModel salary = assets.Pages.GetAtIndex<MoneyModel>(0);
        MoneyModel savings = assets.Pages.GetAtIndex<MoneyModel>(1);
        MoneyModel mortgage = assets.Pages.GetAtIndex<MoneyModel>(2);
        
        Assert.Equal("/form/your-assets/your-salary", salary.Path);
        Assert.Equal("/form/your-assets/your-savings", savings.Path);
        Assert.Equal("/form/your-assets/your-mortgage", mortgage.Path);
    }
}