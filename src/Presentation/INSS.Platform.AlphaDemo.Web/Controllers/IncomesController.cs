using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class IncomesController : BaseFormListController<IncomeModel> 
{
    public IncomesController(IFormCacheClient formCacheClient)
        : base(formCacheClient, "income", nameof(SourceOfIncome)) { }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(SourceOfIncome));
    }

    public IActionResult SourceOfIncome()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> SourceOfIncome(IncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.SourceOfIncome), model.SourceOfIncome, nameof(GrossIncome));
    }

    public IActionResult GrossIncome()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> GrossIncome(IncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.GrossIncome), model.GrossIncome, nameof(PaymentFrequency));
    }

    public IActionResult PaymentFrequency()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> PaymentFrequency(IncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.PaymentFrequency), model.PaymentFrequency, nameof(IncomeProvider));
    }

    public IActionResult IncomeProvider()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> IncomeProvider(IncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.IncomeProvider), model.IncomeProvider, nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        SetFormAsComplete();

        return RedirectToAction("Index", "TaskList");
    }
}


