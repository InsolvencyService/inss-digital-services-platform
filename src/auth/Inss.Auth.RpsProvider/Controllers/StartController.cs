using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.RpsProvider.Controllers;

public class StartController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Used as a keep alive
    }
}