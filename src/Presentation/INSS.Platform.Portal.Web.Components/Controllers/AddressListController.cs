using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class AddressListController : BaseController<AddressListModel>
{
    private readonly IModelService<AddressListModel> _modelService; 

    public AddressListController(IModelService<AddressListModel> addressListService) : base(addressListService)
    {
        _modelService = addressListService;
    }
    
    [HttpGet]
    public override async Task<IActionResult> Index(string? id = null)
    {
        AddressListModel model = await _modelService.LoadAsync(Request.Path.Value);
        model.AppendAddressToList();

        return View(model);
    }
}