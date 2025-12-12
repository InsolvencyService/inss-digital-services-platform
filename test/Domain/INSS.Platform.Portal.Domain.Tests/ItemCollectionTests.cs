using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class ItemCollectionTests
{
    [Fact]
    public void NewRowRequired_CreateNewRow_AppendsPagesToItems()
    {
        ItemCollection items = CreateItemCollection();

        items.CreateNewRow();
        
        Assert.Equal(2, items.Count);
        Assert.Equal(items[0].Count, items[1].Count);
    }
    
    [Fact]
    public void NewRowRequired_CreateNewRow_ReturnsFirstPageOfNewRow()
    {
        ItemCollection items = CreateItemCollection();

        BaseModel firstPage = items.CreateNewRow();
        
        Assert.Equal(items[1][0], firstPage);
    }

    [Fact]
    public void PageChanges_UpdateExistingItem_CopiesModelToExistingPage()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel existingFullName = (FullNameModel)items[0][0];
        FullNameModel fullName = new() { Id = existingFullName.Id, FullName = "Homer Simpson" };
        
        items.UpdateExistingItem(fullName);
        
        Assert.Equal(existingFullName.FullName, fullName.FullName);
    }

    [Fact]
    public void UnknownModel_FindExistingFor_ExistingModel()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel model = new() { Id = Guid.NewGuid().ToString(), PageUrl = "/path/to/page" };

        BaseModel? foundModel = items.FindExistingFor(model);

        Assert.Null(foundModel);
    }
    
    [Fact]
    public void KnownModel_FindExistingFor_ExistingModel()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel existingPage = (FullNameModel)items[0][0];
        FullNameModel model = new() { Id = existingPage.Id, PageUrl = existingPage.PageUrl };

        BaseModel? foundModel = items.FindExistingFor(model);

        Assert.NotNull(foundModel);
        Assert.Equal(existingPage, foundModel);
    }
    
    [Fact]
    public void UnknownModelId_FindExistingFor_ExistingModel()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel model = new() { Id = Guid.NewGuid().ToString(), PageUrl = "/path/to/page" };

        BaseModel? foundModel = items.FindExistingFor(model.Id);

        Assert.Null(foundModel);
    }
    
    [Fact]
    public void KnownModelId_FindExistingFor_ExistingModel()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel existingPage = (FullNameModel)items[0][0];
        FullNameModel model = new() { Id = existingPage.Id, PageUrl = existingPage.PageUrl };

        BaseModel? foundModel = items.FindExistingFor(model.Id);

        Assert.NotNull(foundModel);
        Assert.Equal(existingPage, foundModel);
    }
    
    [Fact]
    public void NoMatchToPages_GetNextPageFor_ReturnsNull()
    {
        ItemCollection items = CreateItemCollection();

        BaseModel? foundModel = items.GetNextPageAfter(new AddressModel(), new AddAnotherModel());

        Assert.Null(foundModel);
    }
    
    [Fact]
    public void FirstPageInList_GetNextPageFor_ReturnsPageAfter()
    {
        ItemCollection items = CreateItemCollection();
        FullNameModel firstPage = (FullNameModel)items[0][0];
        AddressModel secondPage = (AddressModel)items[0][1];

        BaseModel? foundModel = items.GetNextPageAfter(firstPage, new AddAnotherModel());

        Assert.NotNull(foundModel);
        Assert.Equal(secondPage, foundModel);
    }
    
    [Fact]
    public void LastPageInList_GetNextPageFor_ReturnsAddAnother()
    {
        ItemCollection items = CreateItemCollection();
        AddressModel secondPage = (AddressModel)items[0][1];

        BaseModel? foundModel = items.GetNextPageAfter(secondPage, new AddAnotherModel());

        Assert.NotNull(foundModel);
        Assert.IsType<AddAnotherModel>(foundModel);
    }
    
    private static ItemCollection CreateItemCollection()
    {
        return [[new FullNameModel(), new AddressModel()]];
    }
}
