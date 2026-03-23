namespace GovUk.Forms.Domain;

public abstract class GroupPageModel : PageModel
{
    public PageModelList Pages { get; init; } = [];

    public virtual PageModelList SavablePages => Pages;

    public virtual PageModelList SubmittablePages => Pages;
}