using INSS.Platform.Portal.Application.Services;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable Mvc.ViewNotResolved

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class BaseController<T> : Controller
{
    private readonly IModelService<T> _modelService;

    protected BaseController(IModelService<T> modelService)
    {
        _modelService = modelService;
    }

    [HttpGet]
    public virtual async Task<IActionResult> Index(string? id = null)
    {
        T? model = await _modelService.LoadAsync(Request.Path.Value);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(T model)
    {
        await _modelService.ValidateAsync(ModelState, model);

        if (ModelState.IsValid)
        {
            string navigateTo = await _modelService.SaveAsync(Request.Path.Value!, model);
            return Redirect(navigateTo);
        }

        return View(model);
    }
}