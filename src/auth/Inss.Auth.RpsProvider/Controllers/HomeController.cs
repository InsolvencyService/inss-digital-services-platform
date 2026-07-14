using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}