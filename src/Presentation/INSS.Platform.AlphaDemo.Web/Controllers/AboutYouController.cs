using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Domain;
using INSS.Platform.Portal.Web.Components.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class AboutYouController : BaseFormController<AboutYouModel> 
{
    public AboutYouController(IFormCacheClient formCacheClient)
        : base(formCacheClient) { }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(UserFullName));
    }

    public async Task<IActionResult> UserFullName()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> UserFullName(UserFullNameModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.FullName), model, nameof(DateOfBirth));
    }

    public async Task<IActionResult> DateOfBirth()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task <IActionResult> DateOfBirth(DateOfBirthModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.DateOfBirth), model, nameof(UserAddress));
    }

    public async Task<IActionResult> UserAddress()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> UserAddress(UserAddressModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.Address), model, nameof(TelephoneNumber));
    }

    public async Task<IActionResult> TelephoneNumber()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> TelephoneNumber(TelephoneNumberModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.TelephoneNumber), model, nameof(EmailAddress));
    }

    public async Task<IActionResult> EmailAddress()
    {
        return await ViewWithPersistedModelAsync();
    }

    [HttpPost]
    public async Task<IActionResult> EmailAddress(EmailAddressModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.EmailAddress), model, nameof(Summary));
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


