using INSS.Platform.Portal.Domain.Exceptions;
using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class SectionModelCollectionTests
{
    [Fact]
    public void UnknownSectionUrl_GetSectionByUrl_ThrowsException()
    {
        SectionModelCollection sections = CreateSectionModelCollection();

        FormModelException exception = Assert.Throws<FormModelException>(() => sections.GetSectionByUrl("/unknown-section"));
        
        Assert.Equal("Unable to find the section for url /unknown-section.", exception.Message);
    }
    
    [Fact]
    public void KnownSectionUrl_GetSectionByUrl_ReturnsSection()
    {
        SectionModelCollection sections = CreateSectionModelCollection();
        SectionModel firstSection = sections.First();

        SectionModel actualSection = sections.GetSectionByUrl(firstSection.PageUrl);
        
        Assert.Equal(firstSection, actualSection);
    }
    
    [Fact]
    public void PageNotBelongToAddAnother_GetAddAnotherFor_ThrowsException()
    {
        SectionModelCollection sections = CreateSectionModelCollection();
        BaseModel yourDetailsAddress = sections[0].Pages[0];
        
        FormModelException exception = Assert.Throws<FormModelException>(() => sections.GetAddAnotherFor(yourDetailsAddress));
        
        Assert.Equal($"Unable to find the add another model associated to item {yourDetailsAddress.Id}.", exception.Message);
    }
    
    [Fact]
    public void PageBelongsToAddAnother_GetAddAnotherFor_ThrowsException()
    {
        SectionModelCollection sections = CreateSectionModelCollection();
        AddAnotherModel familyAddAnother = (AddAnotherModel)sections[1].Pages[0];
        BaseModel familyAddress = familyAddAnother.Items[0][1];
        
        AddAnotherModel actualAddAnother = sections.GetAddAnotherFor(familyAddress);
        
        Assert.Equal(familyAddAnother, actualAddAnother);
    }

    private static SectionModelCollection CreateSectionModelCollection()
    {
        SectionModelCollection sections =
        [
            new SectionModel
            {
                Title = "Your Details",
                PathName = "your-details",
                Pages =
                [
                    new AddressModel(),
                    new BankAccountModel()
                ]
            },
            new SectionModel
            {
                Title = "About You",
                PathName = "about-you",
                Pages =
                [
                    new AddAnotherModel
                    {
                        Title = "You and your family",
                        PathName = "you-and-your-family",
                        Items = [[new FullNameModel(), new AddressModel()]]
                    }
                ]
            }
        ];
        return sections;
    }
}