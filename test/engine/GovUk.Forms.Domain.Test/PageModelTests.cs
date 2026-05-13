using System.Globalization;
using Xunit;

namespace GovUk.Forms.Domain.Test;

public class PageModelTests
{
    [Fact]
    public void SummaryModel_ClearValues_IgnoresSummary()
    {
        SummaryModel summary = new()
        {
            Overview = [new SummaryModel.SummaryInfo { Title = "Title", ChangeUrl = "/form/", Values = ["1"] }]
        };

        summary.ClearValues();

        Assert.Single(summary.Overview);
    }

    [Fact]
    public void NonSummaryModel_ClearValues_ResetsValues()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        address.ReturnUrl = summary.Path;
        address.SetCompleted();

        address.ClearValues();

        Assert.Empty(address.AddressLine1);
        Assert.Null(address.AddressLine2);
        Assert.Empty(address.TownCity);
        Assert.Null(address.County);
        Assert.Empty(address.Postcode);
        Assert.Null(address.ReturnUrl);
        Assert.Null(address.CompletedDate);
    }

    [Fact]
    public void PopulatedModel_GetSummaryInfo_ReturnsModelSummaryValues()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();

        string[] values = address.GetSummaryInfo();

        Assert.True(values.Contains(address.AddressLine1));
        Assert.False(values.Contains(address.AddressLine2));
        Assert.False(values.Contains(address.TownCity));
        Assert.False(values.Contains(address.County));
        Assert.False(values.Contains(address.Postcode));
    }

    [Fact]
    public void DisplayFormattedModel_GetSummaryInfo_ReturnsFormattedModelValue()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        SalaryModel salary = section.Pages.GetFirstOf<SalaryModel>();

        string[] values = salary.GetSummaryInfo();

        Assert.True(values.Contains(salary.Value.ToString("C", CultureInfo.CurrentCulture)));
    }

    [Fact]
    public void PopulatedModel_CopyTo_UpdatesUnsetVersion()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        AddressModel copyOf = new();
        
        address.CopyTo(copyOf);
        
        Assert.Equal(address.AddressLine1, copyOf.AddressLine1);
        Assert.Equal(address.AddressLine2, copyOf.AddressLine2);
        Assert.Equal(address.TownCity, copyOf.TownCity);
        Assert.Equal(address.County, copyOf.County);
        Assert.Equal(address.Postcode, copyOf.Postcode);
    }

    [Fact]
    public void PopulatedModel_Clone_ReturnsNewInstance()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        TestSectionDefaults.YourDetails(section);
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();

        AddressModel cloneOf = (AddressModel)address.Clone();
        
        Assert.NotEqual(address, cloneOf);
        Assert.Equal(address.AddressLine1, cloneOf.AddressLine1);
        Assert.Equal(address.AddressLine2, cloneOf.AddressLine2);
        Assert.Equal(address.TownCity, cloneOf.TownCity);
        Assert.Equal(address.County, cloneOf.County);
        Assert.Equal(address.Postcode, cloneOf.Postcode);
    }
}