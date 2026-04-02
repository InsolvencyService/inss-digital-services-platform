using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Serialization;
using Xunit;

namespace GovUk.Forms.Domain.Test.Serialization;

public class FormSerializerTests
{
    [Fact]
    public void FromForm_SerializeForm_ContainsSectionInfo()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        string json = FormSerializer.SerializeForm(form);

        Assert.Contains("\"Title\":\"Your Details\"", json);
        Assert.Contains("\"State\":\"NotStarted\"", json);
    }
    
    [Fact]
    public void FromForm_SerializeForm_ContainsFullNamePageInfo()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        string json = FormSerializer.SerializeForm(form);

        Assert.Contains("\"$type\":\"FullNameModel\"", json);
        Assert.Contains("\"Value\":\"\"", json);
        Assert.Contains("\"Title\":\"Your Name\"", json);
        Assert.Contains("\"ReturnUrl\":null", json);
        Assert.Contains("\"Id\":\"", json);
        Assert.Contains("\"Path\":\"/form/your-details/your-fullname\"", json);
        Assert.Contains("\"ViewName\":\"_FullName\"", json);
        Assert.Contains("\"TypeName\":\"GovUk.Forms.Domain.FullNameModel\"", json);
        Assert.Contains("\"EditMode\":\"NotStarted\"", json);
    }
    
    [Fact]
    public void FromForm_SerializeForm_ContainsAddressPageInfo()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        string json = FormSerializer.SerializeForm(form);

        Assert.Contains("\"$type\":\"FullNameModel\"", json);
        Assert.Contains("\"AddressLine1\":\"\"", json);
        Assert.Contains("\"AddressLine2\":null", json);
        Assert.Contains("\"TownCity\":\"\"", json);
        Assert.Contains("\"County\":null", json);
        Assert.Contains("\"Postcode\":\"\"", json);
        Assert.Contains("\"Title\":\"Your Name\"", json);
        Assert.Contains("\"ReturnUrl\":null", json);
        Assert.Contains("\"Id\":\"", json);
        Assert.Contains("\"Path\":\"/form/your-details/your-address\"", json);
        Assert.Contains("\"ViewName\":\"_Address\"", json);
        Assert.Contains("\"TypeName\":\"GovUk.Forms.Domain.AddressModel\"", json);
        Assert.Contains("\"EditMode\":\"NotStarted\"", json);
    }

    [Fact]
    public void FromFormJson_DeserializeForm_ReturnsFormInfo()
    {
        FormModel form = FormSerializer.DeserializeForm(FormJson);
        
        Assert.Equal("/form", form.Path);
        Assert.Equal("_Form", form.ViewName);
        Assert.Equal("GovUk.Forms.Domain.FormModel", form.TypeName);
    }
    
    [Fact]
    public void FromFormJson_DeserializeForm_ReturnsSectionInfo()
    {
        FormModel form = FormSerializer.DeserializeForm(FormJson);
        
        Assert.Single(form.Sections);
        SectionModel section = form.Sections.First();
        Assert.Equal("/form/your-details", section.Path);
        Assert.Equal("Your Details", section.Title);
        Assert.Equal(SectionStateTypes.InProgress, section.State);
    }
    
    [Fact]
    public void FromFormJson_DeserializeForm_ReturnsFullNameInfo()
    {
        FormModel form = FormSerializer.DeserializeForm(FormJson);
        
        Assert.Single(form.Sections);
        SectionModel section = form.Sections.First();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        Assert.Equal("/form/your-details/your-fullname", fullName.Path);
        Assert.Equal("Homer Simpson", fullName.Value);
        Assert.Equal(PageEditTypes.Editing, fullName.EditMode);
    }
    
    [Fact]
    public void FromFormJson_DeserializeForm_ReturnsAddressInfo()
    {
        FormModel form = FormSerializer.DeserializeForm(FormJson);
        
        Assert.Single(form.Sections);
        SectionModel section = form.Sections.First();
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        Assert.Equal("/form/your-details/your-address", address.Path);
        Assert.Equal("101 Ivy Terrace", address.AddressLine1);
        Assert.Equal("Wood Lane", address.AddressLine2);
        Assert.Equal("Treetown", address.TownCity);
        Assert.Equal("Oak County", address.County);
        Assert.Equal("TN33 0DN", address.Postcode);
        Assert.Equal(PageEditTypes.NotStarted, address.EditMode);
    }
    
    [Fact]
    public void FromFullName_SerializePage_ReturnsJsonContainingProperties()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";

        string json = FormSerializer.SerializePage(fullName);

        Assert.Contains("\"$type\":\"FullNameModel\"", json);
        Assert.Contains("\"Value\":\"Homer Simpson\"", json);
        Assert.Contains("\"Title\":\"Your Name\"", json);
        Assert.Contains("\"ReturnUrl\":null", json);
        Assert.Contains("\"Id\":\"", json);
        Assert.Contains("\"Path\":\"/form/your-details/your-fullname\"", json);
        Assert.Contains("\"ViewName\":\"_FullName\"", json);
        Assert.Contains("\"TypeName\":\"GovUk.Forms.Domain.FullNameModel\"", json);
        Assert.Contains("\"EditMode\":\"NotStarted\"", json);
    }
    
    [Fact]
    public void FromFullNameJson_DeserializePage_ReturnsFullName()
    {
        FullNameModel fullName = (FullNameModel)FormSerializer.DeserializePage(FullNamePageJson, typeof(FullNameModel));

        Assert.NotNull(fullName.Id);
        Assert.Equal("Homer Simpson", fullName.Value);
        Assert.Equal("/form/your-details/your-fullname", fullName.Path);
        Assert.Equal("_FullName", fullName.ViewName);
        Assert.Null(fullName.ReturnUrl);
        Assert.Equal(PageEditTypes. NotStarted, fullName.EditMode);
    }
    
    private const string FormJson = @"
        {
          ""Sections"" : [ {
            ""Title"" : ""Your Details"",
            ""State"" : ""InProgress"",
            ""Pages"" : [ {
              ""$type"" : ""FullNameModel"",
              ""Value"" : ""Homer Simpson"",
              ""EditMode"" : ""Editing"",
              ""Title"" : ""Your Name"",
              ""ReturnUrl"" : null,
              ""Id"" : ""70a36a67-4db5-4d11-a4c5-0769497e2221"",
              ""Path"" : ""/form/your-details/your-fullname"",
              ""ViewName"" : ""_FullName"",
              ""TypeName"" : ""GovUk.Forms.Domain.FullNameModel""
            }, {
              ""$type"" : ""AddressModel"",
              ""AddressLine1"" : ""101 Ivy Terrace"",
              ""AddressLine2"" : ""Wood Lane"",
              ""TownCity"" : ""Treetown"",
              ""County"" : ""Oak County"",
              ""Question"" : ""What is your address?"",
              ""Postcode"" : ""TN33 0DN"",
              ""EditMode"" : ""NotStarted"",
              ""Title"" : ""Your Address"",
              ""ReturnUrl"" : null,
              ""Id"" : ""390a79cc-1dad-467e-ac08-d091dc9a44d3"",
              ""Path"" : ""/form/your-details/your-address"",
              ""ViewName"" : ""_Address"",
              ""TypeName"" : ""GovUk.Forms.Domain.AddressModel""
            }],
            ""CurrentNodeId"" : null,
            ""Id"" : ""ab7d8a81-3f2c-415e-980d-6b9417f398ac"",
            ""Path"" : ""/form/your-details"",
            ""ViewName"" : ""_Section"",
            ""TypeName"" : ""GovUk.Forms.Domain.SectionModel""
          } ],
          ""CanSubmit"" : false,
          ""Id"" : ""34fc4a66-ef06-48e1-8610-dcdfd1291e3c"",
          ""Path"" : ""/form"",
          ""ViewName"" : ""_Form"",
          ""TypeName"" : ""GovUk.Forms.Domain.FormModel""
        }";
    
    private const string FullNamePageJson = @"
        {
          ""$type"" : ""FullNameModel"",
          ""Value"" : ""Homer Simpson"",
          ""EditMode"" : ""NotStarted"",
          ""Title"" : ""Your Name"",
          ""ReturnUrl"" : null,
          ""Group"" : null,
          ""Id"" : ""7f141eaa-03ca-4004-bbf8-e34a4bd20acc"",
          ""Path"" : ""/form/your-details/your-fullname"",
          ""ViewName"" : ""_FullName"",
          ""TypeName"" : ""GovUk.Forms.Domain.FullNameModel""
        }";
}