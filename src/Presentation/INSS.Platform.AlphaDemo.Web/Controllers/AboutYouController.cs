using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class AboutYouController : BaseFormController<AboutYouModel> 
{
    public AboutYouController()
    {
        SessionKey = nameof(AboutYouModel);
    }

    public IActionResult Index()
    {
        return RedirectToAction(nameof(FullName));
    }

    public IActionResult FullName()
    {
        LoadFormFromSession();
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> FullName(AboutYouModel model)
    {
        return await Next(model, ModelState, nameof(model.FullName), model.FullName, nameof(DateOfBirth));
    }

    public IActionResult DateOfBirth()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task <IActionResult> DateOfBirth(AboutYouModel model)
    {
        return await Next(model, ModelState, nameof(model.DateOfBirth), model.DateOfBirth, nameof(Address));
    }

    public IActionResult Address()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> Address(AboutYouModel model)
    {
        return await Next(model, ModelState, nameof(model.Address), model.Address, nameof(TelephoneNumber));
    }

    public IActionResult TelephoneNumber()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> TelephoneNumber(AboutYouModel model)
    {
        return await Next(model, ModelState, nameof(model.TelephoneNumber), model.TelephoneNumber, nameof(EmailAddress));

    }

    public IActionResult EmailAddress()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> EmailAddress(AboutYouModel model)
    {
        return await Next(model, ModelState, nameof(model.EmailAddress), model.EmailAddress, nameof(Summary));
    }

    public IActionResult Summary()
    {
        return PopulatedView();
    }

    [HttpPost]
    public async Task<IActionResult> SummaryComplete()
    {
        FormIsComplete();
        return Redirect("/TaskList");
    }
}


