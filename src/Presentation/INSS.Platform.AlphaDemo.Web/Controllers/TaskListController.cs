using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Audit.Application.Users.Handlers;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Events.Domain;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Domain.Abstract;
using INSS.Platform.Shared.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class TaskListController : Controller
{
    private readonly ICanonicalDataClient _canonicalDataClient;
    private readonly IFormCacheClient _formCacheClient;
    private readonly IDomainEventDispatcher _dispatcher;

    public TaskListController(ICanonicalDataClient canonicalDataClientClient, IFormCacheClient formCacheClient, IDomainEventDispatcher dispatcher)
    {
        _canonicalDataClient = canonicalDataClientClient;
        _formCacheClient = formCacheClient;
        _dispatcher = dispatcher;
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

        TaskListViewModel model = new()
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
        return View(model: instanceId);
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

        bool success = await _canonicalDataClient.PostUserDataAsync(userData);
 
        if (success)
        {
            // Demonstrate raising an event from the application layer, in a real application this would likely be raised from the domain layer, for example as part of a domain service that handles the orchestration of the user creation.
            await RaiseAuditEventsAsync(userData, instanceId, currentUser);
        }

        return success;
    }

    /// <summary>
    /// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
    /// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
    /// In a properly defined application the events would be documented and also adhere to a defined contract.
    /// </summary>
    private async Task RaiseAuditEventsAsync(User user, Guid instanceId, string currentUser)
    {
        foreach (BankDetails bankDetails in user.BankDetails)
        {
            AddUserBankDetailsCommand userBankDetailsCommand = new()
            {
                User = currentUser,
                CorrelationId = instanceId,
                AccountName = bankDetails.AccountName,
                SortCode = bankDetails.SortCode
            };

            AddUserBankDetailsHandler.Handle(user, userBankDetailsCommand);
        }

        List<IDomainEvent> events = [.. user.DomainEvents];
        user.ClearDomainEvents();

        await _dispatcher.DispatchAsync(events, HttpContext.RequestAborted);
    }
}
