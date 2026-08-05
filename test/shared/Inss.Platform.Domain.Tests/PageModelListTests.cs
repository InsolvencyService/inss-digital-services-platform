using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Exceptions;
using Xunit;

namespace Inss.Platform.Domain.Tests;

public class PageModelListTests
{
    [Fact]
    public void PageNotExists_GetPage_ThrowsException()
    {
        PageModelList pageModelList = [new PageModel { Path = "/your-name", Title = "Your name" }];

        ComponentException exception = Assert.Throws<ComponentException>(() => pageModelList.GetPage("/your-address"));
        
        Assert.Equal("Cannot get page for path /your-address.", exception.Message);
    }
    
    [Fact]
    public void PageExists_GetPage_ReturnsPage()
    {
        PageModelList pageModelList = [new PageModel { Path = "/your-name", Title = "Your name" }];

        PageModel page = pageModelList.GetPage("/your-name");
        
        Assert.Equal("Your name", page.Title);
    }
    
    [Fact]
    public void PageWithComponentNotExists_GetPage_ThrowsException()
    {
        PageModelList pageModelList =
        [
            new PageModel
            {
                Path = "/your-name", Title = "Your name", Components =
                [
                    new SingleLineTextComponentModel
                    {
                        Id = "YourName", Question = "What is your name?", AssociatedPagePath = "/your-name"
                    }
                ]
            }
        ];

        ComponentException exception = Assert.Throws<ComponentException>(pageModelList.GetFirstPageAssociatedTo<SearchTermComponentModel>);
        
        Assert.Equal("Cannot get page for component SearchTermComponentModel.", exception.Message);
    }
    
    [Fact]
    public void PageWithComponentExists_GetPage_ReturnsPage()
    {
        PageModelList pageModelList =
        [
            new PageModel
            {
                Path = "/your-name", Title = "Your name", Components =
                [
                    new SingleLineTextComponentModel
                    {
                        Id = "YourName", Question = "What is your name", AssociatedPagePath = "/your-name"
                    }
                ]
            }
        ];

        PageModel page = pageModelList.GetFirstPageAssociatedTo<SingleLineTextComponentModel>();
        
        Assert.Equal("Your name", page.Title);
    }
}