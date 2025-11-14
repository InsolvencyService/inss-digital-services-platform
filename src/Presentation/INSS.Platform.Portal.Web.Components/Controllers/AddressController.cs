using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class AddressController : BaseController<AddressModel>
{
    public AddressController(IModelService<AddressModel> addressService) : base(addressService)
    {
    }
}