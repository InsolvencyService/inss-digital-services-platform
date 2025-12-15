using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Moq;
using Xunit;

namespace INSS.Platform.Portal.Application.Tests.Services;

public class FormSerializationServiceTests
{
    private readonly FormSerializationService _service;
    private readonly Mock<IModelTypeService> _mockModelTypeService;

    public FormSerializationServiceTests()
    {
        _mockModelTypeService = new Mock<IModelTypeService>();
        _service = new FormSerializationService(_mockModelTypeService.Object);
    }

    [Fact]
    public void FormWithModels_SerializeAndDeserialize_ChecksFormSerialization()
    {
        _mockModelTypeService.Setup(m => m.GetModelTypes()).Returns([typeof(AddressModel), typeof(BankAccountModel)]);
        FormModel form = CreateFormModel();

        string json = _service.Serialize(form);
        FormModel deserializedForm = _service.Deserialize(json);

        Assert.Equal(form.Id, deserializedForm.Id);
        Assert.Equal(form.PathName, deserializedForm.PathName);
        Assert.Equal(form.PageUrl, deserializedForm.PageUrl);
        Assert.Equal(form.Sections.Count, deserializedForm.Sections.Count); 
        AssertSectionsEqual(form.Sections[0], deserializedForm.Sections[0]);
        Assert.Equal(form.Sections[0].Pages.Count, deserializedForm.Sections[0].Pages.Count);
        AssertAddressModelsEqual((AddressModel)form.Sections[0].Pages[0], (AddressModel)deserializedForm.Sections[0].Pages[0]);
        AssertBankAccountModelsEqual((BankAccountModel)form.Sections[0].Pages[1], (BankAccountModel)deserializedForm.Sections[0].Pages[1]);
    }

    private static void AssertSectionsEqual(SectionModel expected, SectionModel actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.PathName, actual.PathName);
        Assert.Equal(expected.PageUrl, actual.PageUrl);
        Assert.Equal(expected.Pages.Count, actual.Pages.Count);
    }

    private static void AssertAddressModelsEqual(AddressModel expected, AddressModel actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.PathName, actual.PathName);
        Assert.Equal(expected.PageUrl, actual.PageUrl);
        Assert.Equal(expected.AddressLine1, actual.AddressLine1);
        Assert.Equal(expected.AddressLine2, actual.AddressLine2);
        Assert.Equal(expected.TownCity, actual.TownCity);
        Assert.Equal(expected.Postcode, actual.Postcode);
    }

    private static void AssertBankAccountModelsEqual(BankAccountModel expected, BankAccountModel actual)
    {
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.PathName, actual.PathName);
        Assert.Equal(expected.PageUrl, actual.PageUrl);
        Assert.Equal(expected.AccountNumber, actual.AccountNumber);
        Assert.Equal(expected.SortCode, actual.SortCode);
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
                        new AddressModel {
                            AddressLine1 = "123 Test St", 
                            AddressLine2 = "Testville", 
                            TownCity = "Testford",
                            Postcode = "TE5 7ST"
                        },
                        new BankAccountModel { 
                            AccountNumber = "12345678", 
                            SortCode = "12-34-56" 
                        }
                    ]
                }
            ]
        };

        form.Initialize();

        return form;
    }
}
