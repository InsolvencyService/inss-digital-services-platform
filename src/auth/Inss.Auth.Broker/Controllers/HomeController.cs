using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}