using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class BankDetailsController : BaseFormController<BankDetailsModel>
{
    public BankDetailsController(IFormCacheClient formCacheClient)
        : base(formCacheClient) { }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(AccountName));
    }

    public IActionResult AccountName()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> AccountName(BankDetailsModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.AccountName), model.AccountName, nameof(SortCode));
    }

    public IActionResult SortCode()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> SortCode(BankDetailsModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.SortCode), model.SortCode, nameof(AccountNumber));
    }

    public IActionResult AccountNumber()
    {
        return ViewWithPersistedModel();
    }


    [HttpPost]
    public async Task<IActionResult> AccountNumber(BankDetailsModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.AccountNumber), model.AccountNumber, nameof(Summary));
    }

    public IActionResult Summary()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        SetFormAsComplete();

        return RedirectToAction("Index", "TaskList");
    }
}
