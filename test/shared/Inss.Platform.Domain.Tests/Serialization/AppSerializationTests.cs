using Inss.Platform.Domain.Components.Common;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Serialization;
using Xunit;

namespace Inss.Platform.Domain.Tests.Serialization;

public class AppSerializationTests
{
    private readonly AppModel _app = new()
    {
        Session = new SessionId("Test"),
        Pages =
        [
            new PageModel
            {
                Path = "/your-first-name", Title = "Your first name", Components =
                [
                    new SingleLineTextComponentModel
                    {
                        Id = "YourFirstName", Question = "What is your first name?", AssociatedPagePath = "/your-first-name"
                    }
                ]
            },
            new PageModel
            {
                Path = "/your-last-name", Title = "Your last name", Components =
                [
                    new SingleLineTextComponentModel
                    {
                        Id = "YourLastName", Question = "What is your last name?", AssociatedPagePath = "/your-last-name"
                    }
                ]
            }
        ]
    };

    [Fact]
    public void AppWithPages_SerializeAndDeserialize_HydratesAppAndPages()
    {
        string json = AppSerialization.Serialize(_app);
        AppModel app = AppSerialization.Deserialize(json);
        
        Assert.Equal("Test", app.Session);
        Assert.Equal(2, app.Pages.Count);
        Assert.Equal("/your-first-name", app.Pages[0].Path);
        Assert.Equal("Your first name", app.Pages[0].Title);
        Assert.Single(app.Pages[0].Components);
        Assert.Equal("/your-last-name", app.Pages[1].Path);
        Assert.Equal("Your last name", app.Pages[1].Title);
        Assert.Single(app.Pages[1].Components);
    }
}