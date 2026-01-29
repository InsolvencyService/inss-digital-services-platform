using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Web.Components.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class BankDetailsController : BaseFormController<BankDetailsModel>
{
    private readonly IBankClient _bankClient;

    public BankDetailsController(IFormCacheClient formCacheClient, IBankClient bankClient)
        : base(formCacheClient)
    {
        _bankClient = bankClient;
    }

    public async Task<IActionResult> Index()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Index(BankDetailsModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        BankAccountVerificationResponse verificationResponse = await _bankClient.VerifyBankDetailsAsync(
            new BankAccountVerificationRequest
            {
                CustomerName = model.AccountName,
                BankAccount = model.AccountNumber,
                SortCode = model.SortCode,
                AccountType = AccountType.Personal
            });

        if (!verificationResponse.Result)
        {
            switch (verificationResponse.ReasonCode)
            {
                case "NOROUTE":
                case "SCNS":
                    ModelState.AddModelError(nameof(model.SortCode), verificationResponse.ResultText);
                    break;
                case "AC01":
                    ModelState.AddModelError(nameof(model.AccountNumber), verificationResponse.ResultText);
                    break;
                default:
                    ModelState.AddModelError(nameof(model.AccountName), verificationResponse.ResultText);
                    break;
            }
        }

        return await ValidateAndRedirectToNextSectionAsync(model, nameof(Summary));
    }

    public async Task<IActionResult> Summary()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        await SetFormAsCompleteAsync();

        return RedirectToAction("Index", "TaskList");
    }
}
