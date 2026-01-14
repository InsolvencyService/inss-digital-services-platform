using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class BankDetailsController : BaseFormController<BankDetailsModel>
{
    public BankDetailsController()
    {
        SessionKey = nameof(BankDetailsModel);
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(SortCode));
    }

    public IActionResult SortCode()
    {
        LoadFormFromSession();
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> SortCode(BankDetailsModel model)
    {
        return await Next(model, ModelState, nameof(model.SortCode), model.SortCode, nameof(AccountNumber));
    }

    public IActionResult AccountNumber()
    {
        return PopulatedView();
    }


    [HttpPost]
    public async Task<IActionResult> AccountNumber(BankDetailsModel model)
    {
        return await Next(model, ModelState, nameof(model.AccountNumber), model.AccountNumber, nameof(Summary));
    }

    public IActionResult Summary()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        FormIsComplete();
        return Redirect("/TaskList");
    }
}
