using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class ConfirmController : BaseController<ConfirmModel>
{
    public ConfirmController(IModelService<ConfirmModel> confirmService) : base(confirmService)
    {          
    }
}