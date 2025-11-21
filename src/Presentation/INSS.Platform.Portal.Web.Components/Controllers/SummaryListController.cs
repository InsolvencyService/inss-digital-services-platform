using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class SummaryListController : BaseController<SummaryListModel>
{
    private readonly IModelService<SummaryListModel> _modelService; 

    public SummaryListController(IModelService<SummaryListModel> modelService) : base(modelService)
    {
        _modelService = modelService;
    }

    [HttpGet]
    public override async Task<IActionResult> Index(string? id = null)
    {
        if (id is null)
        {
            SummaryListModel? model = await _modelService.LoadAsync(Request.Path.Value);
            return View(model);
        }

        return Redirect(await _modelService.GetPageUrlAsync(Request.Path.Value, id));
    }

    public async Task<IActionResult> Change(string previousPage, string id)
    {
        Console.WriteLine($"Change requested for id: {id}");
        return RedirectToPage(previousPage);
    }
}