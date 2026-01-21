using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Models;
using INSS.Platform.Portal.Domain.Forms;
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

    public IActionResult Index()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> Index(BankDetailsModel model)
    {
        if(!ModelState.IsValid)
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
                case "ANNM":
                case "MBAM":
                    ModelState.AddModelError(nameof(model.AccountName), "The account name provided is not the same as the name held on the Account");
                    break;
                case "NOROUTE":
                    ModelState.AddModelError(nameof(model.SortCode), "The sort code is not valid.");
                    break;
                default:
                    ModelState.AddModelError(nameof(model.AccountNumber), "The account details are not valid.");
                    break;
            }
        }

        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(Summary));
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
