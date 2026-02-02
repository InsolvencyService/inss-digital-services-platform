using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc;
using INSS.Platform.Shared.Web.Utilities;
using INSS.Platform.Portal.Domain.Abstract;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class TaskListController : Controller
{
    private readonly ICanonicalDataClient _canonicalDataClient;
    private readonly IFormCacheClient _formCacheClient;

    public TaskListController(ICanonicalDataClient canonicalDataClientClient, IFormCacheClient formCacheClient)
    {
        _canonicalDataClient = canonicalDataClientClient;
        _formCacheClient = formCacheClient;
    }

    public async Task<IActionResult> Index(string? status, bool submissionError = false)
    {
        AboutYouModel? aboutYou = null;
        BankDetailsModel? bankDetails = null;
        IncomeListModel? incomeList = null;

        if (status is not null and "new")
        {
            HttpContext.Session.Clear();
        }
        else
        {
            aboutYou = await _formCacheClient.GetFormFromCacheAsync<AboutYouModel>();
            bankDetails = await _formCacheClient.GetFormFromCacheAsync<BankDetailsModel>();
            incomeList = await _formCacheClient.GetFormListFromCacheAsync<IncomeListModel>();
        }

        TaskListViewModel model = new ()
        {
            AboutYouCompleted = IsFormComplete(aboutYou),
            BankDetailsCompleted = IsFormComplete(bankDetails),
            IncomesCompleted = IsFormComplete(incomeList),
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
        AboutYouModel aboutYou = await _formCacheClient.GetFormFromCacheAsync<AboutYouModel>();
        BankDetailsModel bankDetails = await _formCacheClient.GetFormFromCacheAsync<BankDetailsModel>();
        IncomeListModel incomeList = await _formCacheClient.GetFormListFromCacheAsync<IncomeListModel>();
        string currentUser = User.Identity?.Name ?? "Claim not set";

        User userData = new()
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            FullName = aboutYou.FullName.Value,
            DateOfBirth = aboutYou.DateOfBirth.Value.GetValueOrDefault(),
            EmailAddress = aboutYou.EmailAddress.Value,
            TelephoneNumber = aboutYou.TelephoneNumber.Value,
            CreatedBy = currentUser,
        };

        userData.Addresses.Add(new Address
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            AddressLine1 = aboutYou.Address.AddressLine1,
            AddressLine2 = aboutYou.Address.AddressLine2,
            TownCity = aboutYou.Address.TownCity,
            County = aboutYou.Address.County,
            Postcode = aboutYou.Address.Postcode,
            CreatedBy = currentUser
        });

        userData.BankDetails.Add(new BankDetails
        {
            InstanceId = instanceId,
            Id = aboutYou.Id,
            AccountName = bankDetails.AccountName,
            SortCode = bankDetails.SortCode,
            AccountNumber = bankDetails.AccountNumber,
            BuildingSocietyRollNumber = bankDetails.BuildingSocietyRollNumber,
            CreatedBy = currentUser
        });

        foreach (IncomeModel income in incomeList.Items)
        {
            userData.Incomes.Add(new Income
            {
                InstanceId = instanceId,
                Id = income.Id,
                SourceOfIncome = income.SourceOfIncome.Value?.Description() ?? string.Empty,
                GrossIncome = income.GrossIncome.Value.GetValueOrDefault(),
                PaymentFrequency = income.PaymentFrequency.Value?.Description() ?? string.Empty,
                IncomeProvider = income.IncomeProvider.Value ?? string.Empty,
                CreatedBy = currentUser
            });
        }

        return await _canonicalDataClient.PostUserDataAsync(userData);
    }
}
