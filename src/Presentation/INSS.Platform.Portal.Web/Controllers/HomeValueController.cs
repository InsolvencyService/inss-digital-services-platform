using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Web.Components.Controllers;
using INSS.Platform.Portal.Web.Models;

namespace INSS.Platform.Portal.Web.Controllers;

public class HomeValueController : BaseController<HomeValueModel>
{
    public HomeValueController(IModelService<HomeValueModel> modelService) : base(modelService)
    {
    }
}