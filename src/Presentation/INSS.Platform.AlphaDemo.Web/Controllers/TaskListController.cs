using INSS.Platform.AlphaDemo.Web.Helpers;
using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class TaskListController : Controller
{
    private readonly IFormApiClient _formApiClient;

    public TaskListController(IFormApiClient formApiClient)
    {
        _formApiClient = formApiClient;
    }

    public IActionResult Index(string? status, bool submissionError = false)
    {
        AboutYouModel? aboutYou = null;
        BankDetailsModel? bankDetails = null;

        if (status is not null and "new")
        {
            HttpContext.Session.Clear();
        }
        else
        {
            aboutYou = FormSessionHelper.LoadFormFromSession<AboutYouModel>(HttpContext, nameof(AboutYouModel));
            bankDetails = FormSessionHelper.LoadFormFromSession<BankDetailsModel>(HttpContext, nameof(BankDetailsModel));
        }

        TaskListViewModel model = new ()
        {
            AboutYouCompleted = IsFormComplete(aboutYou),
            BankDetailsCompleted = IsFormComplete(bankDetails),
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
        AboutYouModel aboutYou = FormSessionHelper.LoadFormFromSession<AboutYouModel>(HttpContext, nameof(AboutYouModel))!;
        BankDetailsModel bankDetails = FormSessionHelper.LoadFormFromSession<BankDetailsModel>(HttpContext, nameof(BankDetailsModel))!;

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
            AccountName = "Me",
            AccountNumber = bankDetails.AccountNumber,
            SortCode = bankDetails.SortCode.Replace("-", string.Empty)
        });

        return await _formApiClient.PostFormUserDataAsync(userData);
    }
}
