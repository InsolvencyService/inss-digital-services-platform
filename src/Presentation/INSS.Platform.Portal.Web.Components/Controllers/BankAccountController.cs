using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Domain;

namespace INSS.Platform.Portal.Web.Components.Controllers;

public class BankAccountController : BaseController<BankAccountModel>
{
    public BankAccountController(IModelService<BankAccountModel> bankAccountService) : base(bankAccountService)
    {          
    }
}