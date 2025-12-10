using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Moq;
using Xunit;

namespace INSS.Platform.Portal.Application.Tests.Services;

public class FormServiceTests
{
    private readonly FormService _service;
    private readonly Mock<IFormStateService> _mockFormStateService;
    private readonly Mock<IFormModelFactory> _mockFormModelFactory;

    public FormServiceTests()
    {
        _mockFormStateService = new Mock<IFormStateService>();
        _mockFormModelFactory = new Mock<IFormModelFactory>();
        _service = new FormService(_mockFormStateService.Object, _mockFormModelFactory.Object);
    }

    [Fact]
    public async Task FormDoesNotExist_GetAsync_CreatesNewFormAndReturnsPath()
    {
        _mockFormStateService.Setup(s => s.FormExistsAsync()).ReturnsAsync(false);
        FormModel newForm = CreateFormModel();
        _mockFormModelFactory.Setup(f => f.CreateAsync()).ReturnsAsync(newForm);

        BaseModel result = await _service.GetAsync("/test-form");

        _mockFormStateService.Verify(s => s.SaveAsync(newForm), Times.Once);
        Assert.Equal(newForm.FindPage("/test-form"), result);
    }

    [Fact]
    public async Task FormDoesExist_GetAsync_ReturnsPathForExistingForm()
    {
        _mockFormStateService.Setup(s => s.FormExistsAsync()).ReturnsAsync(true);
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);

        BaseModel result = await _service.GetAsync("/test-form");

        Assert.Equal(existingForm.FindPage("/test-form"), result);
    }

    [Fact]
    public async Task ConfirmModel_SaveAsync_ReturnsSummaryListPageUrl()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        string itemId = GetListItemId(existingForm);
        ConfirmModel confirmModel = new() { Id = itemId, Question = "Confirm save?" };

        string pageUrl = await _service.SaveAsync(confirmModel);

        Assert.Equal("/test-form/your-details/summary-list", pageUrl);
    }

    [Fact]
    public async Task SectionModel_SaveAsync_MarksSectionComplete()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        SectionModel section = existingForm.Sections[0];

        string pageUrl = await _service.SaveAsync(section);

        Assert.True(section.IsComplete);
    }

    [Fact]
    public async Task SomeModel_SaveAsync_CopiesValuesToSource()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        BankAccountModel bankAccount = new()
        {
            PageUrl = "/test-form/your-details/bank-account",
            AccountNumber = "12345678",
            SortCode = "12-34-56"
        };

        string pageUrl = await _service.SaveAsync(bankAccount);

        BankAccountModel existingBankAccount = (BankAccountModel)existingForm.FindPage(bankAccount.PageUrl);
        Assert.Equal(bankAccount.AccountNumber, existingBankAccount.AccountNumber);
        Assert.Equal(bankAccount.SortCode, existingBankAccount.SortCode);
    }

    [Fact]
    public async Task LastModel_SaveAsync_ReturnsSummaryPageUrl()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        BankAccountModel bankAccount = new()
        {
            PageUrl = "/test-form/your-details/bank-account",
            AccountNumber = "12345678",
            SortCode = "12-34-56"
        };

        string pageUrl = await _service.SaveAsync(bankAccount);

        Assert.Equal("/test-form/your-details/summary", pageUrl);
    }

    [Fact]
    public async Task NotTheLastModel_SaveAsync_ReturnsNextPageUrl()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        AddressModel address = new()
        {
            PageUrl = "/test-form/your-details/address",
            AddressLine1 = "123 Test St", 
            AddressLine2 = "Testville", 
            TownCity = "Testford",
            Postcode = "TE5 7ST"
        };

        string pageUrl = await _service.SaveAsync(address);

        Assert.Equal("/test-form/your-details/summary-list", pageUrl);
    }

    [Fact]
    public async Task ChangingItemInSummaryList_ChangeAsync_ReturnsPreviousPageUrl()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        string itemId = GetListItemId(existingForm);

        string previousPageUrl = await _service.ChangeAsync(itemId);

        Assert.Equal("/test-form/your-details/address", previousPageUrl);
    }

    [Fact]
    public async Task RemovingItemInSummaryList_RemoveAsync_ReturnsConfirmModelWithId()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        string itemId = GetListItemId(existingForm);

        ConfirmModel confirm = await _service.RemoveAsync(itemId);

        Assert.Equal(itemId, confirm.Id);
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
                        new SummaryListModel { Items = [new AddressModel {Id = Guid.NewGuid().ToString("D")}] },
                        new BankAccountModel()
                    ]
                }
            ]
        };

        form.Initialize();

        return form;
    }

    private static string GetListItemId(FormModel form)
    {
        SummaryListModel summaryList = (SummaryListModel)form.Sections[0].Pages[1];
        return summaryList.Items[0].Id;
    }
}
