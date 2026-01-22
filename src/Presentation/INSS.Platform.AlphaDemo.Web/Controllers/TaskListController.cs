using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class TaskListController : Controller
{
    private readonly IFormApiClient _formApiClient;
    private readonly IFormCacheClient _formCacheClient;

    public TaskListController(IFormApiClient formApiClient, IFormCacheClient formCacheClient)
    {
        _formApiClient = formApiClient;
        _formCacheClient = formCacheClient;
    }

    public IActionResult Index(string? status, bool submissionError = false)
    {
        AboutYouModel? aboutYou = null;
        BankDetailsModel? bankDetails = null;
        List<IncomeModel>? incomes = null;

        if (status is not null and "new")
        {
            HttpContext.Session.Clear();
        }
        else
        {
            aboutYou = _formCacheClient.GetFormFromCache<AboutYouModel>(_formCacheClient.GetFormCacheKey<AboutYouModel>());
            bankDetails = _formCacheClient.GetFormFromCache<BankDetailsModel>(_formCacheClient.GetFormCacheKey<BankDetailsModel>());
            incomes = _formCacheClient.GetFormListFromCache<IncomeModel>(_formCacheClient.GetFormCacheKey<IncomeModel>());
        }

        TaskListViewModel model = new ()
        {
            AboutYouCompleted = IsFormComplete(aboutYou),
            BankDetailsCompleted = IsFormComplete(bankDetails),
            IncomesCompleted = incomes is not null && incomes.Count > 0 && incomes.All(IsFormComplete),
            SubmissionError = submissionError
        };

        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> SubmitForms()
    {
        Guid instanceId = Guid.NewGuid();
        bool postSuccess = await PostFormDataAsync(instanceId);

        if (postSuccess)
        {
            return RedirectToAction(nameof(Complete), new { instanceId = instanceId.ToString("N").ToLowerInvariant() });
        }
        else
        {
            return RedirectToAction(nameof(Index), new { submissionError = true });
        }
    }

    public IActionResult Complete(string instanceId)
    {
        return View(model :instanceId);
    }

    private static bool IsFormComplete<T>(T? form) where T : FormBase
    {
        return form != null && form.IsComplete;
    }

    private async Task<bool> PostFormDataAsync(Guid instanceId)
    {
        AboutYouModel aboutYou = _formCacheClient.GetFormFromCache<AboutYouModel>(_formCacheClient.GetFormCacheKey<AboutYouModel>())!;
        BankDetailsModel bankDetails = _formCacheClient.GetFormFromCache<BankDetailsModel>(_formCacheClient.GetFormCacheKey<BankDetailsModel>())!;
        List<IncomeModel> incomes = _formCacheClient.GetFormListFromCache<IncomeModel>(_formCacheClient.GetFormCacheKey<IncomeModel>())!;

        User userData = new()
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            FullName = aboutYou.FullName,
            DateOfBirth = aboutYou.DateOfBirth.GetValueOrDefault(),
            EmailAddress = aboutYou.EmailAddress,
            TelephoneNumber = aboutYou.TelephoneNumber,
        };

        userData.Addresses.Add(new Address
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            AddressLine1 = aboutYou.Address.AddressLine1,
            AddressLine2 = aboutYou.Address.AddressLine2,
            TownCity = aboutYou.Address.TownCity,
            County = aboutYou.Address.County,
            Postcode = aboutYou.Address.Postcode
        });

        userData.BankDetails.Add(new BankDetails
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            AccountName = bankDetails.AccountName,
            SortCode = bankDetails.SortCode,
            AccountNumber = bankDetails.AccountNumber,
            BuildingSocietyRollNumber = bankDetails.BuildingSocietyRollNumber
        });

        foreach (IncomeModel income in incomes)
        {
            userData.Incomes.Add(new Income
            {
                InstanceId = instanceId,
                Id = income.Id,
                SourceOfIncome = income.SourceOfIncome.ToString() ?? string.Empty,
                GrossIncome = income.GrossIncome.GetValueOrDefault(),
                PaymentFrequency = income.PaymentFrequency.ToString() ?? string.Empty,
                IncomeProvider = income.IncomeProvider
            });
        }

        return await _formApiClient.PostFormUserDataAsync(userData);
    }
}
