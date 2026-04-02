using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.HostApp.Controllers;

public class StartController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}