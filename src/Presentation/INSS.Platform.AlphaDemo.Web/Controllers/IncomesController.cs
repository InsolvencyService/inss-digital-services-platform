using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class IncomesController : BaseFormListController<IncomeModel> 
{
    private readonly IncomeModel _incomeModel = new();

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
    public async Task<IActionResult> SourceOfIncome(SourceOfIncomeModel model)
    {
        _incomeModel.SourceOfIncome = model;
        return await ValidateAndRedirectToNextSectionAsync(_incomeModel, ModelState, nameof(_incomeModel.SourceOfIncome), _incomeModel.SourceOfIncome, nameof(GrossIncome));
    }

    public IActionResult GrossIncome()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> GrossIncome(GrossIncomeModel model)
    {
        _incomeModel.GrossIncome = model;
        return await ValidateAndRedirectToNextSectionAsync(_incomeModel, ModelState, nameof(_incomeModel.GrossIncome), _incomeModel.GrossIncome, nameof(PaymentFrequency));
    }

    public IActionResult PaymentFrequency()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> PaymentFrequency(PaymentFrequencyModel model)
    {
        _incomeModel.PaymentFrequency = model;
        return await ValidateAndRedirectToNextSectionAsync(_incomeModel, ModelState, nameof(_incomeModel.PaymentFrequency), _incomeModel.PaymentFrequency, nameof(IncomeProvider));
    }

    public IActionResult IncomeProvider()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> IncomeProvider(IncomeProviderModel model)
    {
        _incomeModel.IncomeProvider = model;
        return await ValidateAndRedirectToNextSectionAsync(_incomeModel, ModelState, nameof(_incomeModel.IncomeProvider), _incomeModel.IncomeProvider, nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        SetFormAsComplete();

        return RedirectToAction("Index", "TaskList");
    }
}


