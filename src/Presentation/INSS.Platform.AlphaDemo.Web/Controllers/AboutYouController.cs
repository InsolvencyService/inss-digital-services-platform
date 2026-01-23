using INSS.Platform.AlphaDemo.Web.Models;
using INSS.Platform.AlphaDemo.Web.Services;
using INSS.Platform.Portal.Domain.Forms;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class AboutYouController : BaseFormController<AboutYouModel> 
{
    private readonly AboutYouModel _aboutYouModel = new ();

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
    public async Task<IActionResult> FullName(FullNameModel model)
    {
        _aboutYouModel.FullName = model;
        return await ValidateAndRedirectToNextSectionAsync(_aboutYouModel, ModelState, nameof(_aboutYouModel.FullName), _aboutYouModel.FullName, nameof(DateOfBirth));
    }

    public IActionResult DateOfBirth()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task <IActionResult> DateOfBirth(DateOfBirthModel model)
    {
        _aboutYouModel.DateOfBirth = model;
        return await ValidateAndRedirectToNextSectionAsync(_aboutYouModel, ModelState, nameof(_aboutYouModel.DateOfBirth), _aboutYouModel.DateOfBirth, nameof(UserAddress));
    }

    public IActionResult UserAddress()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> UserAddress(AddressModel model)
    {
        _aboutYouModel.Address = model;
        return await ValidateAndRedirectToNextSectionAsync(_aboutYouModel, ModelState, nameof(_aboutYouModel.Address), _aboutYouModel.Address, nameof(TelephoneNumber));
    }

    public IActionResult TelephoneNumber()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> TelephoneNumber(TelephoneNumberModel model)
    {
        _aboutYouModel.TelephoneNumber = model;
        return await ValidateAndRedirectToNextSectionAsync(_aboutYouModel, ModelState, nameof(_aboutYouModel.TelephoneNumber), _aboutYouModel.TelephoneNumber, nameof(EmailAddress));

    }

    public IActionResult EmailAddress()
    {
        return ViewWithPersistedModel();
    }

    [HttpPost]
    public async Task<IActionResult> EmailAddress(EmailAddressModel model)
    {
        _aboutYouModel.EmailAddress = model;
        return await ValidateAndRedirectToNextSectionAsync(_aboutYouModel, ModelState, nameof(_aboutYouModel.EmailAddress), _aboutYouModel.EmailAddress, nameof(Summary));
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


