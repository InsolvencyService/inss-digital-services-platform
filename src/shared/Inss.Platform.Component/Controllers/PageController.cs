using Inss.Platform.Application.Services;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Primitives;
using Inss.Platform.Domain.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Inss.Platform.Component.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class PageController : Controller
{
    private readonly IPageService _pageService;
    private const string TempDataKey = "ErrorModel";

    public PageController(IPageService pageService)
    {
        _pageService = pageService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (TempData[TempDataKey] is string pageErrorJson)
        {
            PageModel errorPage = AppSerialization.DeserializePage(pageErrorJson);

            if (errorPage.PageValidationInfo is not null)
            {
                foreach (var error in errorPage.PageValidationInfo.Errors)
                {
                    ModelState.AddModelError(error.Properties[0], error.Message);
                }

                TempData.Clear();
                return View(errorPage);
            }
        }
        
        QueryParamList queryParams = GetQueryParams();
        PagePath requestPath = new(Request.Path);
        PageModel page = await _pageService.LoadAsync(requestPath, queryParams);
        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PageModel page)
    {
        QueryParamList queryParams = GetQueryParams();
        PageModel? validatedPage = await _pageService.ValidateAsync(page);

        if (validatedPage is not null)
        {
            TempData[TempDataKey] = AppSerialization.SerializePage(validatedPage);
            return Redirect(validatedPage.Path + queryParams.BuildQueryParams());
        }

        PagePath? redirectTo = await _pageService.SaveAsync(page);
        return Redirect(redirectTo ?? "/");
    }
    
    private QueryParamList GetQueryParams()
    {
        QueryParamList queryParams = [];

        foreach (KeyValuePair<string, StringValues> queryParam in Request.Query)
        {
            string? value = queryParam.Value.Count > 0 ? queryParam.Value[0] : null;
            queryParams[queryParam.Key] = value;
        }

        return queryParams;
    }
}