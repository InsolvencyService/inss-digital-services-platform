using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Web.Components.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class IncomesController : BaseFormListController<IncomeListModel, IncomeModel> 
{
    public IncomesController(IFormCacheClient formCacheClient)
        : base(formCacheClient, "income", nameof(SourceOfIncome)) { }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(SourceOfIncome));
    }

    public async Task<IActionResult> SourceOfIncome()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> SourceOfIncome(SourceOfIncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(SourceOfIncome), model, nameof(GrossIncome));
    }

    public async Task<IActionResult> GrossIncome()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> GrossIncome(GrossIncomeModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(GrossIncome), model, nameof(PaymentFrequency));
    }

    public async Task<IActionResult> PaymentFrequency()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> PaymentFrequency(PaymentFrequencyModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(PaymentFrequency), model, nameof(IncomeProvider));
    }

    public async Task<IActionResult> IncomeProvider()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> IncomeProvider(IncomeProviderModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(IncomeProvider), model, nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        await SetFormAsCompleteAsync();

        return RedirectToAction("Index", "TaskList");
    }
}
