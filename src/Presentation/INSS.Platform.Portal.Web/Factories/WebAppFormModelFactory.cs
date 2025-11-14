using INSS.Platform.Portal.Application.Factories;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Web.Models;

namespace INSS.Platform.Portal.Web.Factories;

public sealed class WebAppFormModelFactory : IFormModelFactory
{
    public Task<FormModel> CreateAsync()
    {
        var form = new FormModel();
        PrefabModelSections.AddYourDetails(form);

        var section = new SectionModel { Name = "Assets", PathName = "assets" };
        form.AddSection(section);
        section.AddPage(new BankAccountModel());
        section.AddPage(new HomeValueModel());

        var sectionAboutYou = new SectionModel { Name = "About You", PathName = "about-you" };
        form.AddSection(sectionAboutYou);
        sectionAboutYou.AddPage(new FullNameModel { Title = "Full Name"});
        sectionAboutYou.AddPage(new AddressModel { Title = "Address"});

        form.Initialize();

        /*
        // Example serialize and deserialize
        var json = form.Serialize();
        
        var form2 = FormModel.Deserialize(json);
        */

        return Task.FromResult(form);
    }
}