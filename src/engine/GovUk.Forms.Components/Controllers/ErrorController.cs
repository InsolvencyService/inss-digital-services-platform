using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.Components.Controllers;

public class ErrorController : Controller
{
    public IActionResult Index()
    { 
        return View();
    }
    
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatus(int statusCode)
    {
        ViewBag.StatusCode = statusCode;

        return statusCode switch
        {
            404 => View("NotFound"),
            _   => View("Index")
        };
    }
}