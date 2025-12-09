using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;
using HomeValueModel = INSS.Platform.Portal.Web.Models.HomeValueModel;

namespace INSS.Platform.Portal.Web.Factories;

public sealed class WebAppFormModelFactory : IFormModelFactory
{
    public Task<FormModel> CreateAsync()
    {
        FormModel form = new()
        {
            Sections =
            [
                new SectionModel
                {
                    Name = "Your Details",
                    PathName = "your-details",
                    Pages =
                    [
                        new AddressModel(),
                        new BankAccountModel()
                    ]
                },
                new SectionModel
                {
                    Name = "Assets",
                    PathName = "assets",
                    Pages =
                    [
                        new BankAccountModel(),
                        new HomeValueModel()
                    ]
                },
                new SectionModel
                {
                    Name = "About You",
                    PathName = "about-you",
                    Pages =
                    [
                        new FullNameModel(),
                        new AddressModel(),
                        new SummaryListModel
                        {
                            PathName = "address-list", 
                            Name = "Address List", 
                            RemoveQuestionText = "Are you sure yuo want to remove the address?"
                        }
                    ]
                }
            ]
        };

        form.Initialize();
        
        return Task.FromResult(form);
    }
}