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
        newForm.CurrentPageId = newForm.Sections[0].Pages[0].Id;
        _mockFormModelFactory.Setup(f => f.CreateAsync()).ReturnsAsync(newForm);

        BaseModel result = await _service.GetAsync("/test-form");

        _mockFormStateService.Verify(s => s.SaveAsync(newForm), Times.Once);
        Assert.Equal(newForm.FindPage(newForm.CurrentPageId), result);
    }

    [Fact]
    public async Task FormDoesExist_GetAsync_ReturnsPathForExistingForm()
    {
        _mockFormStateService.Setup(s => s.FormExistsAsync()).ReturnsAsync(true);
        FormModel existingForm = CreateFormModel();
        existingForm.CurrentPageId = existingForm.Sections[0].Pages[0].Id;
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);

        BaseModel result = await _service.GetAsync("/test-form");

        _mockFormStateService.Verify(s => s.SaveAsync(existingForm), Times.Never);
        Assert.Equal(existingForm.FindPage(existingForm.CurrentPageId), result);
    }
    
    [Fact]
    public async Task FormDoesExist_StartAsync_ReturnsPathForExistingForm()
    {
        _mockFormStateService.Setup(s => s.FormExistsAsync()).ReturnsAsync(true);
        FormModel existingForm = CreateFormModel();
        existingForm.CurrentPageId = existingForm.Sections[0].Pages[0].Id;
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);

        BaseModel result = await _service.StartAsync("/test-form/your-details/summary/start");

        _mockFormStateService.Verify(s => s.SaveAsync(existingForm), Times.Once);
        Assert.Equal(existingForm.FindPage(existingForm.CurrentPageId), result);
    }

    [Fact]
    public async Task ConfirmModel_SaveAsync_ReturnsAddAnotherPage()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        string itemId = GetListItemId(existingForm);
        ConfirmModel confirmModel = new() { Id = itemId, Question = "Confirm save?" };

        BaseModel page = await _service.SaveAsync(confirmModel);

        Assert.Equal("/test-form/your-details/your-family", page.PageUrl);
    }

    [Fact]
    public async Task SectionModel_SaveAsync_MarksSectionComplete()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        SectionModel section = existingForm.Sections[0];

        await _service.SaveAsync(section);

        Assert.True(section.IsComplete);
    }

    [Fact]
    public async Task BankAccountModelChanges_SaveAsync_CopiesValuesToSource()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        BankAccountModel bankAccount = new()
        {
            Id = existingForm.Sections[0].Pages[2].Id,
            PageUrl = "/test-form/your-details/bank-account",
            AccountNumber = "12345678",
            SortCode = "12-34-56"
        };

        await _service.SaveAsync(bankAccount);

        BankAccountModel existingBankAccount = (BankAccountModel)existingForm.FindPage(bankAccount.Id);
        Assert.Equal(bankAccount.AccountNumber, existingBankAccount.AccountNumber);
        Assert.Equal(bankAccount.SortCode, existingBankAccount.SortCode);
    }

    [Fact]
    public async Task LastModel_SaveAsync_ReturnsSummary()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        BankAccountModel bankAccount = new()
        {
            Id = existingForm.Sections[0].Pages[2].Id,
            PageUrl = "/test-form/your-details/bank-account",
            AccountNumber = "12345678",
            SortCode = "12-34-56"
        };

        BaseModel page = await _service.SaveAsync(bankAccount);

        Assert.IsType<SectionModel>(page);
    }

    [Fact]
    public async Task NotTheLastModel_SaveAsync_ReturnsNextPage()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        AddressModel address = new()
        {
            Id = existingForm.Sections[0].Pages[0].Id,
            PageUrl = "/test-form/your-details/address",
            AddressLine1 = "123 Test St", 
            AddressLine2 = "Testville", 
            TownCity = "Testford",
            Postcode = "TE5 7ST"
        };

        BaseModel page = await _service.SaveAsync(address);

        Assert.IsType<AddAnotherModel>(page);
    }

    [Fact]
    public async Task ChangingItem_ChangeAsync_ReturnsPageToChange()
    {
        FormModel existingForm = CreateFormModel();
        _mockFormStateService.Setup(f => f.GetAsync()).ReturnsAsync(existingForm);
        string itemId = GetListItemId(existingForm);

        BaseModel pageToChange = await _service.ChangeAsync(itemId);

        Assert.IsType<FullNameModel>(pageToChange);
    }

    [Fact]
    public async Task RemovingItem_RemoveAsync_ReturnsConfirmModelWithId()
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
                    Title = "Your Details",
                    PathName = "your-details",
                    Pages =
                    [
                        new AddressModel(),
                        new AddAnotherModel
                        {
                            Title = "Your family",
                            PathName = "your-family",
                            Items = [[new FullNameModel(), new AddressModel()]]
                        },
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
        AddAnotherModel addAnotherModel = (AddAnotherModel)form.Sections[0].Pages[1];
        return addAnotherModel.Items[0][0].Id;
    }
}
