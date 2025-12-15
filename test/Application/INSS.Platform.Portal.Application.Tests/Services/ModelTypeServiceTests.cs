using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Xunit;

namespace INSS.Platform.Portal.Application.Tests.Services;

public class ModelTypeServiceTests
{
    [Fact]
    public void UnknownModelType_GetModelType_ThrowsException()
    {
        ModelTypeService service = new(typeof(AddressModel).Assembly);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => service.GetModelType("NonExistentModel"));

        Assert.Equal("Unable to find model type for NonExistentModel.", exception.Message);
    }

    [Fact]
    public void KnownModelType_GetModelType_ReturnsType()
    {
        ModelTypeService service = new(typeof(AddressModel).Assembly);

        Type modelType = service.GetModelType("AddressModel");

        Assert.Equal(typeof(AddressModel), modelType);
    }

    [Fact]
    public void WithAssemblyOfTypes_GetModelTypes_ReturnsList()
    {
        ModelTypeService service = new(typeof(AddressModel).Assembly);

        IEnumerable<Type> modelTypes = service.GetModelTypes().ToList();

        // Assert some as this list will grow, and we don't want to have to update this test every time a new model is added
        Assert.Contains(typeof(AddressModel), modelTypes);
        Assert.Contains(typeof(BankAccountModel), modelTypes);
        Assert.Contains(typeof(FullNameModel), modelTypes);
    }
}
