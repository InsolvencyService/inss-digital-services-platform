using INSS.Platform.Portal.Domain.Exceptions;
using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class FormModelTests
{
    [Fact]
    public void UnknownPage_GetCurrentPageFor_ReturnsForm()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsAddress = form.Sections[0].Pages[0];
        BaseModel model = new AddressModel { Id = yourDetailsAddress.Id, PageUrl = "/unknown-path" };

        BaseModel actualPage = form.GetCurrentPageFor(model);
        
        Assert.Equal(form, actualPage);
    }
    
    [Fact]
    public void KnownPage_GetCurrentPageFor_ReturnsFormVersion()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsAddress = form.Sections[0].Pages[0];
        BaseModel model = new AddressModel { Id = yourDetailsAddress.Id, PageUrl = yourDetailsAddress.PageUrl };

        BaseModel actualPage = form.GetCurrentPageFor(model);
        
        Assert.Equal(yourDetailsAddress, actualPage);
    }
    
    [Fact]
    public void KnownAddAnotherPage_GetCurrentPageFor_ReturnsFormVersion()
    {
        FormModel form = CreateFormModel();
        AddAnotherModel addAnother = (AddAnotherModel)form.Sections[2].Pages[0];
        BaseModel familyAddress = addAnother.Items[0][1];
        BaseModel model = new AddressModel { Id = familyAddress.Id, PageUrl = familyAddress.PageUrl };

        BaseModel actualPage = form.GetCurrentPageFor(model);
        
        Assert.Equal(familyAddress, actualPage);
    }
    
    [Fact]
    public void UnknownPageId_GetPage_ReturnsForm()
    {
        FormModel form = CreateFormModel();
        string unknownPageUrl = Guid.NewGuid().ToString("D");

        BaseModel page = form.FindPage(unknownPageUrl);
        
        Assert.Equal(form, page);
    }
    
    [Fact]
    public void KnownPageId_GetPage_ReturnsPage()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsAddress = form.Sections[0].Pages[0];

        BaseModel actualPage = form.FindPage(yourDetailsAddress.Id);
        
        Assert.Equal(yourDetailsAddress, actualPage);
    }
    
    [Fact]
    public void KnownSectionId_GetPage_ReturnsSection()
    {
        FormModel form = CreateFormModel();
        SectionModel yourDetailsSection = form.Sections[0];

        BaseModel actualPage = form.FindPage(yourDetailsSection.Id);
        
        Assert.Equal(yourDetailsSection, actualPage);
    }
    
    [Fact]
    public void KnownAddAnotherId_GetPage_ReturnsAddAnother()
    {
        FormModel form = CreateFormModel();
        AddAnotherModel familyAddAnother = (AddAnotherModel)form.Sections[2].Pages[0];

        BaseModel actualPage = form.FindPage(familyAddAnother.Id);
        
        Assert.Equal(familyAddAnother, actualPage);
    }
    
    [Fact]
    public void KnownAddAnotherItemId_GetPage_ReturnsAddAnotherItem()
    {
        FormModel form = CreateFormModel();
        AddAnotherModel familyAddAnother = (AddAnotherModel)form.Sections[2].Pages[0];
        BaseModel item = familyAddAnother.Items[0][1];

        BaseModel actualItem = form.FindPage(item.Id);
        
        Assert.Equal(item, actualItem);
    }

    [Fact]
    public void UnknownAddAnotherId_GetAddAnother_ThrowsException()
    {
        FormModel form = CreateFormModel();
        string unknownId = Guid.NewGuid().ToString("D");

        FormModelException exception = Assert.Throws<FormModelException>(() => form.GetAddAnother(unknownId));
        
        Assert.Equal($"Unable to find the add another model associated to item {unknownId}.", exception.Message);
    }
    
    [Fact]
    public void KnownAddAnotherId_GetAddAnother_ReturnsAddAnother()
    {
        FormModel form = CreateFormModel();
        AddAnotherModel familyAddAnother = (AddAnotherModel)form.Sections[2].Pages[0];

        AddAnotherModel actualAddAnother = form.GetAddAnother(familyAddAnother.Id);
        
        Assert.Equal(familyAddAnother, actualAddAnother);
    }
    
    [Fact]
    public void UnknownPageUrl_FindNextPageAfter_ReturnsForm()
    {
        FormModel form = CreateFormModel();
        
        BaseModel nextPage = form.FindNextPageAfter(new AddressModel());

        Assert.NotNull(nextPage);
        Assert.Equal(form, nextPage);
    }

    [Fact]
    public void PageUrlMatchesFirstPage_FindNextPageAfter_ReturnsNextPageInSection()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsAddress = form.Sections[0].Pages[0];
        
        BaseModel nextPage = form.FindNextPageAfter(yourDetailsAddress);

        Assert.NotNull(nextPage);
        Assert.Equal(form.Sections[0].Pages[1], nextPage);
    }

    [Fact]
    public void PageUrlMatchesLastPage_FindNextPageAfter_ReturnsSection()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsBankAccount = form.Sections[0].Pages[1];
        
        BaseModel nextPage = form.FindNextPageAfter(yourDetailsBankAccount);

        Assert.NotNull(nextPage);
        Assert.Equal(form.Sections[0], nextPage);
    }
    
    private static FormModel CreateFormModel()
    {
        FormModel form = new()
        {
            PathName = "test-form",
            Sections =
            [
                new SectionModel
                {
                    Name = "Your Details",
                    PathName = "your-details",
                    Pages =
                    [
                        new AddressModel(),
                        new BankAccountModel()
                    ]
                },
                new SectionModel
                {
                    Name = "Assets",
                    PathName = "assets",
                    Pages =
                    [
                        new BankAccountModel { PathName = "bank-account" }
                    ]
                },
                new SectionModel
                {
                    Name = "About You",
                    PathName = "about-you",
                    Pages =
                    [
                        new AddAnotherModel
                        {
                            Name = "You and your family",
                            PathName = "you-and-your-family",
                            Items = [[new FullNameModel(), new AddressModel()]]
                        }
                    ]
                }
            ]
        };

        form.Initialize();

        return form;
    }
}