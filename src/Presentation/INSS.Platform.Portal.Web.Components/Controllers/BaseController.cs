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
    public async Task<IActionResult> Index()
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

    [HttpGet]
    public async Task<IActionResult> Change(string id)
    {
        return Redirect(await _modelService.GetPageUrlAsync(Request.Path.Value!.Replace("/change/", ""), id));
    }

    [HttpGet]
    public async Task<IActionResult> Remove(string id)
    {
        return Redirect(await _modelService.GetRemovedPageUrlAsync(Request.Path.Value!.Replace("/remove/", ""), id));
    }

    [HttpGet]
    public async Task<IActionResult> PostRemove(string id)
    {
        string path = Request.Path.Value!.Replace("/post-remove/", "");
        return Redirect(await _modelService.GetPostRemovedPageUrlAsync(path, id));
    }
}