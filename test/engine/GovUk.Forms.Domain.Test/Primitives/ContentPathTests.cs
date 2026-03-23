using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace GovUk.Forms.Domain.Test.Primitives;

public class ContentPathTests
{
    [Fact]
    public void PathIsRoot_GetRoot_ReturnsPath()
    {
        ContentPath path = "/test";

        ContentPath rootPath = path.GetRoot();
        
        Assert.Equal("/test", rootPath);
    }
    
    [Fact]
    public void PathSubPath_GetRoot_ReturnsPath()
    {
        ContentPath path = "/test/section";

        ContentPath rootPath = path.GetRoot();
        
        Assert.Equal("/test", rootPath);
    }
}