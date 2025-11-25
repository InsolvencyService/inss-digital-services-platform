using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Application.Services;

public class ConfirmService : BasePageModelService<ConfirmModel>
{
    private readonly IFormStateService _formStateService;
    //private readonly IJourneyService _journeyService;
    private readonly IUserSessionResolver _userSessionResolver;

    public ConfirmService(
        IFormStateService  formStateService, 
        IJourneyService  journeyService,
        IUserSessionResolver userSessionResolver)
        : base(formStateService, journeyService, userSessionResolver)
    {
        _formStateService = formStateService;
        //_journeyService = journeyService;
        _userSessionResolver = userSessionResolver;
    }
    
    public override async Task<string> SaveAsync(string requestPath, ConfirmModel model)
    {
        FormModel form = await _formStateService.GetAsync(_userSessionResolver.GetUserId());
        ConfirmModel page = form.FindPage<ConfirmModel>(requestPath);
        if (model.Confirm)
        {
            form.RemovePageModel(page.ConfirmationId!);

            // Find the section that contains this confirm page
            SectionModel section = form.FindSectionForPage(page.PageUrl);
            SummaryListModel? summaryList = (SummaryListModel?)section.GetPreviousPage(page.PageUrl);

            if (summaryList?.Pages.Length == 0)
            {
                page.NextPageUrl = summaryList.PreviousPageUrl;
            }

            // For the section find the previous summary list page

            // Get

            await _formStateService.SaveAsync(_userSessionResolver.GetUserId(), form);           
        }

        return page.NextPageUrl;
    }
}