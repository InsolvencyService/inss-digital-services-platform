using System.Text.Json;
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

        JsonDocument doc = JsonDocument.Parse(json);
        Assert.True(FindPropertyValues(doc.RootElement, "title", "Your Details"));
        Assert.True(FindPropertyValues(doc.RootElement, "state", "NotStarted"));
    }
    
    [Fact]
    public void FromForm_SerializeForm_ContainsFullNamePageInfo()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        string json = FormSerializer.SerializeForm(form);
        
        JsonDocument doc = JsonDocument.Parse(json);
        Assert.True(FindPropertyValues(doc.RootElement, "$type", "FullNameModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "value", ""));
        Assert.True(FindPropertyValues(doc.RootElement, "title", "Your Name"));
        Assert.True(FindPropertyValues(doc.RootElement, "returnUrl", null));
        Assert.True(FindPropertyValues(doc.RootElement, "path", "/form/your-details/your-fullname"));
        Assert.True(FindPropertyValues(doc.RootElement, "viewName", "_FullName"));
        Assert.True(FindPropertyValues(doc.RootElement, "typeName", "GovUk.Forms.Domain.FullNameModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "editMode", "NotStarted"));
    }
    
    [Fact]
    public void FromForm_SerializeForm_ContainsAddressPageInfo()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        
        string json = FormSerializer.SerializeForm(form);

        JsonDocument doc = JsonDocument.Parse(json);
        Assert.True(FindPropertyValues(doc.RootElement, "$type", "AddressModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "addressLine1", ""));
        Assert.True(FindPropertyValues(doc.RootElement, "addressLine2", null));
        Assert.True(FindPropertyValues(doc.RootElement, "townCity", ""));
        Assert.True(FindPropertyValues(doc.RootElement, "county", null));
        Assert.True(FindPropertyValues(doc.RootElement, "postcode", ""));
        Assert.True(FindPropertyValues(doc.RootElement, "title", "Your Name"));
        Assert.True(FindPropertyValues(doc.RootElement, "returnUrl", null));
        Assert.True(FindPropertyValues(doc.RootElement, "path", "/form/your-details/your-address"));
        Assert.True(FindPropertyValues(doc.RootElement, "viewName", "_FullName"));
        Assert.True(FindPropertyValues(doc.RootElement, "typeName", "GovUk.Forms.Domain.AddressModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "editMode", "NotStarted"));
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

        JsonDocument doc = JsonDocument.Parse(json);
        Assert.True(FindPropertyValues(doc.RootElement, "$type", "FullNameModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "value", "Homer Simpson"));
        Assert.True(FindPropertyValues(doc.RootElement, "returnUrl", null));
        Assert.True(FindPropertyValues(doc.RootElement, "path", "/form/your-details/your-fullname"));
        Assert.True(FindPropertyValues(doc.RootElement, "viewName", "_FullName"));
        Assert.True(FindPropertyValues(doc.RootElement, "typeName", "GovUk.Forms.Domain.FullNameModel"));
        Assert.True(FindPropertyValues(doc.RootElement, "editMode", "NotStarted"));
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
    
    private static bool FindPropertyValues(JsonElement element, string propertyName, string? propertyValue)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
            {
                foreach (var prop in element.EnumerateObject())
                {
                    if (prop.NameEquals(propertyName))
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Null && propertyValue is null)
                        {
                            return true;
                        }

                        if (prop.Value.ToString() == propertyValue)
                        {
                            return true;
                        }

                    }
                    else
                    {
                        if (FindPropertyValues(prop.Value, propertyName, propertyValue))
                        {
                            return true;
                        }
                    }
                }

                break;
            }
            case JsonValueKind.Array:
            {
                foreach (var item in element.EnumerateArray())
                {
                    if (FindPropertyValues(item, propertyName, propertyValue))
                    {
                        return true;
                    }
                }

                break;
            }
        }

        return false;
    }
    
    private const string FormJson = @"
        {
          ""sections"" : [ {
            ""title"" : ""Your Details"",
            ""state"" : ""InProgress"",
            ""pages"" : [ {
              ""$type"" : ""FullNameModel"",
              ""value"" : ""Homer Simpson"",
              ""editMode"" : ""Editing"",
              ""title"" : ""Your Name"",
              ""returnUrl"" : null,
              ""id"" : ""70a36a67-4db5-4d11-a4c5-0769497e2221"",
              ""path"" : ""/form/your-details/your-fullname"",
              ""viewName"" : ""_FullName"",
              ""typeName"" : ""GovUk.Forms.Domain.FullNameModel""
            }, {
              ""$type"" : ""AddressModel"",
              ""addressLine1"" : ""101 Ivy Terrace"",
              ""addressLine2"" : ""Wood Lane"",
              ""townCity"" : ""Treetown"",
              ""county"" : ""Oak County"",
              ""question"" : ""What is your address?"",
              ""postcode"" : ""TN33 0DN"",
              ""editMode"" : ""NotStarted"",
              ""title"" : ""Your Address"",
              ""returnUrl"" : null,
              ""id"" : ""390a79cc-1dad-467e-ac08-d091dc9a44d3"",
              ""path"" : ""/form/your-details/your-address"",
              ""viewName"" : ""_Address"",
              ""typeName"" : ""GovUk.Forms.Domain.AddressModel""
            }],
            ""id"" : ""ab7d8a81-3f2c-415e-980d-6b9417f398ac"",
            ""path"" : ""/form/your-details"",
            ""viewName"" : ""_Section"",
            ""typeName"" : ""GovUk.Forms.Domain.SectionModel""
          } ],
          ""canSubmit"" : false,
          ""id"" : ""34fc4a66-ef06-48e1-8610-dcdfd1291e3c"",
          ""path"" : ""/form"",
          ""viewName"" : ""_Form"",
          ""typeName"" : ""GovUk.Forms.Domain.FormModel""
        }";
    
    private const string FullNamePageJson = @"
        {
          ""$type"" : ""FullNameModel"",
          ""value"" : ""Homer Simpson"",
          ""editMode"" : ""NotStarted"",
          ""title"" : ""Your Name"",
          ""returnUrl"" : null,
          ""group"" : null,
          ""id"" : ""7f141eaa-03ca-4004-bbf8-e34a4bd20acc"",
          ""path"" : ""/form/your-details/your-fullname"",
          ""viewName"" : ""_FullName"",
          ""typeName"" : ""GovUk.Forms.Domain.FullNameModel""
        }";
}