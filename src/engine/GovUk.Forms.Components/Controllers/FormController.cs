using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Components.Authentication;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.Components.Controllers;

[DynamicAuthorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class FormController : Controller
{
    private readonly IFormService _formService;

    public FormController(IFormService formService)
    {
        _formService = formService;
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? state = null)
    {
        (ContentModel? Content, ContentPath? RedirectTo) result = await _formService.LoadAsync(new ContentPath(Request.Path), state);
        ViewData.AddBackButton(result.Content is PageModel { PreviousPagePath: not null } page ? page.PreviousPagePath.Value : null);
        ViewData.AddFullWidthLayout(result.Content?.FullWidthLayout == true);
        return result.RedirectTo is not null ? Redirect(result.RedirectTo) : View(result.Content);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ContentModel postedContent)
    {
        ValidationResult[] validationResults = await _formService.ValidateAsync(postedContent);

        if (validationResults.Length > 0)
        {
            foreach (ValidationResult error in validationResults)
            {
                foreach (string memberName in error.MemberNames)
                {
                    ModelState.AddModelError(memberName, error.ErrorMessage ?? string.Empty);
                }
            }

            ViewData.AddBackButton(postedContent is PageModel { PreviousPagePath: not null } page ? page.PreviousPagePath.Value : null);
            ViewData.AddFullWidthLayout(postedContent.FullWidthLayout);
            return View(postedContent);
        }

        ContentPath redirectTo = await _formService.SaveAsync(postedContent);
        return Redirect(redirectTo);
    }
    
    [HttpGet]
    public IActionResult LogOut()
    {
        return SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
    }
}