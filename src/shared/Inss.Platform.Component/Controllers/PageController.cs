using Inss.Platform.Application.Services;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Inss.Platform.Component.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class PageController : Controller
{
    private readonly IPageService _pageService;

    public PageController(IPageService pageService)
    {
        _pageService = pageService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        /*
        
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
        */
        Dictionary<string, string?> queryParams = GetQueryParams();
        PagePath requestPath = new(Request.Path);
        Page page = await _pageService.LoadAsync(requestPath, queryParams);
        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Page page)
    {
        /*
        ValidationResult[] validationResults = await _formService.ValidateAsync(postedContent);

        if (validationResults.Length > 0)
        {
            return Redirect(postedContent.Path);
        }

        ContentPath redirectTo = await _formService.SaveAsync(postedContent);
        return Redirect(redirectTo);
        */
        return await Task.FromResult(Redirect("/"));
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