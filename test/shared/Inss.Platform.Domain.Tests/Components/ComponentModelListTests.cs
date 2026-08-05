using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Exceptions;
using Xunit;

namespace Inss.Platform.Domain.Tests.Components;

public class ComponentModelListTests
{
    [Fact]
    public void UnknownComponentId_GetComponent_ThrowsException()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "YourName", Question = "What is your name?", AssociatedPagePath = "/your-name"
            }
        ];
        
        ComponentException exception = Assert.Throws<ComponentException>(() => components.GetComponent("YourAddress"));
        
        Assert.Equal("Cannot get component for Id YourAddress.", exception.Message);
    }
    
    [Fact]
    public void KnownComponentId_GetComponent_ReturnsComponent()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "YourName", Question = "What is your name?", AssociatedPagePath = "/your-name"
            }
        ];

        ComponentModel singleLineText = components.GetComponent("YourName");
        
        Assert.NotNull(singleLineText);
    }
    
    [Fact]
    public void SameComponentTypeExists_GetFirstOf_ReturnsFirstInstance()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "FirstName", Question = "What is your first name?", AssociatedPagePath = "/your-first-name"
            },
            new SingleLineTextComponentModel
            {
                Id = "LastName", Question = "What is your last name?", AssociatedPagePath = "/your-last-name"
            }
        ];

        SingleLineTextComponentModel singleLineText = components.GetFirstOf<SingleLineTextComponentModel>();
        
        Assert.Equal("FirstName", singleLineText.Id);
        Assert.Equal("/your-first-name", singleLineText.AssociatedPagePath);
    }
    
    [Fact]
    public void ComponentTypeNotExists_GetFirstOf_ThrowsException()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "FirstName", Question = "What is your first name?", AssociatedPagePath = "/your-first-name"
            },
            new SingleLineTextComponentModel
            {
                Id = "LastName", Question = "What is your last name?", AssociatedPagePath = "/your-last-name"
            }
        ];
        
        ComponentException exception = Assert.Throws<ComponentException>(components.GetFirstOf<SearchTermComponentModel>);
        
        Assert.Equal("Unable to find component of type SearchTermComponentModel.", exception.Message);
    }
    
    [Fact]
    public void ComponentExists_HasComponent_ReturnsTrue()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "FirstName", Question = "What is your first name?", AssociatedPagePath = "/your-first-name"
            },
            new SingleLineTextComponentModel
            {
                Id = "LastName", Question = "What is your last name?", AssociatedPagePath = "/your-last-name"
            }
        ];

        bool exists = components.HasComponent<SingleLineTextComponentModel>();
        
        Assert.True(exists);
    }
    
    [Fact]
    public void ComponentNotExists_HasComponent_ReturnsFalse()
    {
        ComponentModelList components = 
        [
            new SingleLineTextComponentModel
            {
                Id = "FirstName", Question = "What is your first name?", AssociatedPagePath = "/your-first-name"
            },
            new SingleLineTextComponentModel
            {
                Id = "LastName", Question = "What is your last name?", AssociatedPagePath = "/your-last-name"
            }
        ];

        bool exists = components.HasComponent<SearchTermComponentModel>();
        
        Assert.False(exists);
    }
}