using INSS.Platform.Portal.Domain.Exceptions;
using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class FormModelTests
{
    [Fact]
    public void UnknownPageUrl_FindPage_ThrowsException()
    {
        FormModel form = CreateFormModel();
        
        FormModelException exception = Assert.Throws<FormModelException>(() => form.FindPage("/path/to/unknown/page"));
        
        Assert.Equal("Unable to find the page associated with /path/to/unknown/page.", exception.Message);
    }

    [Fact]
    public void KnownPageUrl_FindPage_ReturnsPage()
    {
        FormModel form = CreateFormModel();
        
        BaseModel page = form.FindPage("/test-form/assets/bank-account");
        
        Assert.NotNull(page);
    }

    [Fact]
    public void UnknownPageUrlSummaryListItem_FindSummaryList_ThrowsException()
    {
        string unknownItemId = Guid.NewGuid().ToString("D");
        FormModel form = CreateFormModel();
        SummaryListModel summaryList = (SummaryListModel)form.Sections[2].Pages[2];
        summaryList.Items = [new FullNameModel { Id = Guid.NewGuid().ToString("D"), FullName = "Homer Simpson" }];
        
        FormModelException exception = Assert.Throws<FormModelException>(() => form.FindSummaryList(unknownItemId));
        
        Assert.Equal($"Unable to find the summary list for the specified item {unknownItemId}.", exception.Message);
    }

    [Fact]
    public void KnownPageUrlSummaryListItem_FindSummaryList_ThrowsException()
    {
        string itemId = Guid.NewGuid().ToString("D");
        FormModel form = CreateFormModel();
        SummaryListModel summaryList = (SummaryListModel)form.Sections[2].Pages[2];
        summaryList.Items = [new FullNameModel { Id = itemId, FullName = "Homer Simpson" }];
        
        BaseModel item = form.FindSummaryList(itemId);
        
        Assert.NotNull(item);
    }

    [Fact]
    public void FirstPageInSection_FindPageBefore_ThrowsException()
    {
        string unknownItemId = Guid.NewGuid().ToString("D");
        FormModel form = CreateFormModel();
        BaseModel currentPage = form.Sections[0].Pages[0]; // AddressModel
        
        FormModelException exception = Assert.Throws<FormModelException>(() => form.FindPageBefore(currentPage));
        
        Assert.Equal($"Unable to find the page before {currentPage.PageUrl}.", exception.Message);
    }

    [Fact]
    public void KnownPageUrl_FindPageBefore_ReturnsPreviousPage()
    {
        FormModel form = CreateFormModel();
        BaseModel currentPage = form.Sections[0].Pages[1]; // BankAccountModel
        
        BaseModel item = form.FindPageBefore(currentPage);
        
        Assert.NotNull(item);
        Assert.IsType<AddressModel>(item);
    }

    [Fact]
    public void UnknownPageUrl_GetNextPageAfter_ReturnsForm()
    {
        FormModel form = CreateFormModel();
        
        BaseModel nextPage = form.GetNextPageAfter("/path/to/unknown/page");

        Assert.NotNull(nextPage);
        Assert.Equal(form, nextPage);
    }

    [Fact]
    public void PageUrlMatchesFirstPage_GetNextPageAfter_ReturnsNextPageInSection()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsAddress = form.Sections[0].Pages[0];
        
        BaseModel nextPage = form.GetNextPageAfter(yourDetailsAddress.PageUrl);

        Assert.NotNull(nextPage);
        Assert.Equal(form.Sections[0].Pages[1], nextPage);
    }

    [Fact]
    public void PageUrlMatchesLastPage_GetNextPageAfter_ReturnsSection()
    {
        FormModel form = CreateFormModel();
        BaseModel yourDetailsBankAccount = form.Sections[0].Pages[1];
        
        BaseModel nextPage = form.GetNextPageAfter(yourDetailsBankAccount.PageUrl);

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
                        new FullNameModel(),
                        new AddressModel(),
                        new SummaryListModel { PathName = "address-list", Name = "Address List"}
                    ]
                }
            ]
        };

        form.Initialize();

        return form;
    }
}
