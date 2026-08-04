using Microsoft.AspNetCore.Mvc;

namespace Inss.Auth.Broker.Controllers;

public class StartController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Used as a keep alive
    }
}