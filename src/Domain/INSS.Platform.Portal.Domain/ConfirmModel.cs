using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public class ConfirmModel : BaseModel
{
    public ConfirmModel()
    {
        PathName = "confirm";
        Title = "Confirm";
    }

    public string? Question { get; init; }
    
    [Required(ErrorMessage = "Choose Yes or No to confirm")]
    public bool Confirmed { get; init; }

    public BaseModel HandleConfirmation(FormModel form)
    {
        BaseModel page = form.FindPage(Id);
        
        AddAnotherModel addAnother = form.Sections.GetAddAnotherFor(page);
        
        BaseModel? nextPage;
        
        if (Confirmed)
        {
            PageCollection pages = addAnother.Items.First(li => li.Any(i => i.Id == page.Id));
            
            if (addAnother.Items.Count > 1)
            {
                addAnother.Items.Remove(pages);
                nextPage = addAnother;
            }
            else
            {
                foreach (BaseModel item in pages)
                {
                    item.Reset();
                }   
            
                nextPage = addAnother.Items[0][0];
            }
        }
        else
        {
            nextPage = addAnother;
        }

        form.CurrentPageId = nextPage.Id;

        return nextPage;
    }
}