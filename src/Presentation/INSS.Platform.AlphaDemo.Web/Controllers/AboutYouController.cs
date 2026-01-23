using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.Portal.Application.Services;
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

    public IActionResult UserFullName()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> UserFullName(UserFullNameModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.FullName), model, nameof(DateOfBirth));
    }

    public IActionResult DateOfBirth()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task <IActionResult> DateOfBirth(DateOfBirthModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.DateOfBirth), model, nameof(UserAddress));
    }

    public IActionResult UserAddress()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> UserAddress(UserAddressModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.Address), model, nameof(TelephoneNumber));
    }

    public IActionResult TelephoneNumber()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> TelephoneNumber(TelephoneNumberModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.TelephoneNumber), model, nameof(EmailAddress));
    }

    public IActionResult EmailAddress()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> EmailAddress(EmailAddressModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(nameof(AboutYouModel.EmailAddress), model, nameof(Summary));
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


