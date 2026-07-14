using GovUk.Forms.Components.Binding;
using GovUk.Forms.Components.Resolvers;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Components.Test.Binding;

public class DefaultContentBinderTests
{
    [Fact]
    public void PopulatedAddressFormCollection_BindAndReturnModel_ReturnsAddressModel()
    {
        string addressTypeName = typeof(AddressModel).FullName!;
        ITypeNameResolver typeNameResolver = Substitute.For<ITypeNameResolver>();
        typeNameResolver.Resolve(addressTypeName).Returns(typeof(AddressModel));
        Dictionary<string, StringValues> values = new()
        {
            { "Id.Value", "18FAA111-1C48-4681-A206-88AEC8CC1CCF" },
            { "Path.Value", "/test/section/address" },
            { "TypeName", addressTypeName },
            { "ViewName", "_Address" },
            { "AddressLine1", "101 Ivy Terrace" },
            { "AddressLine2", "Wood Lane" },
            { "TownCity", "Treetown" },
            { "County", "Oak County" },
            { "Postcode", "TN33 0DN" }
        };
        IFormCollection formCollection = new FormCollection(values);
        DefaultContentBinder binder = new(typeNameResolver);

        ContentModel content = binder.BindAndReturnModel(addressTypeName, formCollection);
        
        Assert.NotNull(content);
        Assert.Equal(values["Id.Value"], content.Id.Value);
        Assert.Equal(values["Path.Value"], content.Path.Value);
        Assert.IsType<AddressModel>(content);
        AddressModel address = content.As<AddressModel>();
        Assert.Equal(values["AddressLine1"], address.AddressLine1);
        Assert.Equal(values["AddressLine2"], address.AddressLine2);
        Assert.Equal(values["TownCity"], address.TownCity);
        Assert.Equal(values["County"], address.County);
        Assert.Equal(values["Postcode"], address.Postcode);
    }
}