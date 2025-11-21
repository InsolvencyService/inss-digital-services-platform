using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class SummaryListController : BaseController<SummaryListModel>
{
    public SummaryListController(IModelService<SummaryListModel> modelService) : base(modelService)
    {
    }

    public async Task<IActionResult> Change(string previousPage, string id)
    {
        Console.WriteLine($"Change requested for id: {id}");
        return RedirectToPage(previousPage);
    }
}