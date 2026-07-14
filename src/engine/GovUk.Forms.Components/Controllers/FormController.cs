using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Components.Authentication;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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
        Dictionary<string, string?> queryParams = GetQueryParams();
        ContentPath requestPath = new(Request.Path);
        (ContentModel? Content, ContentPath? RedirectTo, PageValidationError[]? ValidationErrors) result = 
            await _formService.LoadAsync(requestPath, queryParams);

        if (result.ValidationErrors?.Length > 0)
        {
            foreach (PageValidationError error in result.ValidationErrors)
            {
                ModelState.AddModelError(error.Properties[0], error.Message);
            }
        }
        
        return result.RedirectTo is not null ? Redirect(result.RedirectTo) : View(result.Content);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContentModel postedContent)
    {
        ValidationResult[] validationResults = await _formService.ValidateAsync(postedContent);

        if (validationResults.Length > 0)
        {
            return Redirect(postedContent.Path);
        }

        ContentPath redirectTo = await _formService.SaveAsync(postedContent);
        return Redirect(redirectTo);
    }
    
    [HttpGet]
    public IActionResult LogOut()
    {
        return SignOut(OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private Dictionary<string, string?> GetQueryParams()
    {
        Dictionary<string, string?> queryParams = [];

        foreach (KeyValuePair<string, StringValues> queryParam in Request.Query)
        {
            string? value = queryParam.Value.Count > 0 ? queryParam.Value[0] : null;
            queryParams[queryParam.Key] = value;
        }

        return queryParams;
    }
}