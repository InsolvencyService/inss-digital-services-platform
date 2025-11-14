namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Below is an example of how we can create a whole section upfront and share it with other projects.
///
/// We can a set of pre-built sections that can be easily attached to a form with all the pages defined.
/// </summary>
public static class PrefabModelSections
{
    public static void AddYourDetails(FormModel form)
    {
        var section = new SectionModel { Name = "Your Details", PathName = "your-details" };
        form.AddSection(section);
        section.AddPage(new AddressModel { Title = "Address" });
        section.AddPage(new BankAccountModel { Title = "Bank Account" });
    }
}