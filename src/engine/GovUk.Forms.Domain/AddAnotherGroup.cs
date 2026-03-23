namespace GovUk.Forms.Domain;

public sealed class AddAnotherGroup : GroupPageModel
{
    public PageModelList WorkingPages
    {
        get
        {
            PageModelList workingPages = [];
            workingPages.AddRange(Pages.Where(p => p is not CheckAnswersModel && p is not AddAnotherModel && p is not RemoveModel));
            return workingPages;
        }
    }

    public override PageModelList SavablePages => [AddAnother];
    
    public override PageModelList SubmittablePages => AddAnother.Items;

    public CheckAnswersModel CheckAnswers => (CheckAnswersModel)Pages.First(p => p is CheckAnswersModel);
    
    public AddAnotherModel AddAnother => (AddAnotherModel)Pages.First(p => p is AddAnotherModel);

    public RemoveModel Remove => (RemoveModel)Pages.First(p => p is RemoveModel);
}