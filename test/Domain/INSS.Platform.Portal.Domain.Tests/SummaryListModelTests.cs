using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class SummaryListModelTests
{
    [Fact]
    public void FromModel_GetChangeUrl_ReturnsExpectedUrl()
    {
        AddressModel address = new() { Id = Guid.NewGuid().ToString("D") };
        SummaryListModel summaryList = new() { PageUrl = "/test/about-you/address" };

        string changeUrl = summaryList.GetChangeUrl(address);

        Assert.Equal("/test/about-you/address/change/?itemId=" + address.Id, changeUrl);
    }

    [Fact]
    public void FromModel_GetRemoveUrl_ReturnsExpectedUrl()
    {
        AddressModel address = new() { Id = Guid.NewGuid().ToString("D") };
        SummaryListModel summaryList = new() { PageUrl = "/test/about-you/address" };

        string changeUrl = summaryList.GetRemoveUrl(address);

        Assert.Equal("/test/about-you/address/remove/?itemId=" + address.Id, changeUrl);
    }
}
