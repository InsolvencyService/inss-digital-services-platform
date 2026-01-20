using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class AboutYouController : BaseFormController<AboutYouModel> 
{
    public AboutYouController(IFormCacheClient formCacheClient)
        : base(formCacheClient) { }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(FullName));
    }

    public IActionResult FullName()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> FullName(AboutYouModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.FullName), model.FullName, nameof(DateOfBirth));
    }

    public IActionResult DateOfBirth()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task <IActionResult> DateOfBirth(AboutYouModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.DateOfBirth), model.DateOfBirth, nameof(Address));
    }

    public IActionResult Address()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> Address(AboutYouModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.Address), model.Address, nameof(TelephoneNumber));
    }

    public IActionResult TelephoneNumber()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> TelephoneNumber(AboutYouModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.TelephoneNumber), model.TelephoneNumber, nameof(EmailAddress));

    }

    public IActionResult EmailAddress()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> EmailAddress(AboutYouModel model)
    {
        return await ValidateAndRedirectToNextSectionAsync(model, ModelState, nameof(model.EmailAddress), model.EmailAddress, nameof(Summary));
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


