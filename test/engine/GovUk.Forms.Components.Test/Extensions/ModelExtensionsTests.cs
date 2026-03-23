using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Components.Test.Extensions;

public class ModelExtensionsTests
{
    [Fact]
    public void ContentIsNotPage_IsPageModel_ReturnsFalse()
    {
        SectionModel section = new();

        bool result = section.IsPageModel();
        
        Assert.False(result);
    }
    
    [Fact]
    public void ContentIsPage_IsPageModel_ReturnsTrue()
    {
        FullNameModel fullName = new();

        bool result = fullName.IsPageModel();
        
        Assert.True(result);
    }
/*
    [Fact]
    public void NotAddAnother_ShowRemoveView_ReturnsFalse()
    {
        FullNameModel fullName = new();

        bool result = fullName.ShowRemoveView();
        
        Assert.False(result);
    }
    
    [Fact]
    public void AddAnotherWithNoItemsToRemove_ShowRemoveView_ReturnsFalse()
    {
        AddAnotherModel addAnother = new();

        bool result = addAnother.ShowRemoveView();
        
        Assert.False(result);
    }
    
    [Fact]
    public void AddAnotherWithItemsToRemove_ShowRemoveView_ReturnsTrue()
    {
        AddAnotherModel addAnother = new() { SetIndex = 1 };//RemoveIdentifiers = ["1234"] };

        bool result = addAnother.ShowRemoveView();
        
        Assert.True(result);
    }
    */
}