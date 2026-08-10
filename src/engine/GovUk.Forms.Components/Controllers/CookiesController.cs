using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.Components.Controllers;

public class CookiesController : Controller
{
    [HttpGet("/cookies")]
    public IActionResult Index()
    {
        return View();
    }    
}