using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.Web.Controllers;

[Authorize]
public class RolesController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
