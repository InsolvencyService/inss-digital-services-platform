using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
