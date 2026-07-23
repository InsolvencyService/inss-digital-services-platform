using GovUk.Forms.Components.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Components.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }
    
    public IActionResult Index()
    { 
        IExceptionHandlerPathFeature? exceptionHandler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandler is not null)
        {
            string path = exceptionHandler.Path;
            string message = exceptionHandler.Error.ToString();
            _logger.UnexpectedError(path, message);
        }
        
        return View();
    }
    
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatus(int statusCode)
    {
        string path = Request.Headers.Referer.Count > 0 && !string.IsNullOrWhiteSpace(Request.Headers.Referer[0]) 
            ? Request.Headers.Referer[0]! 
            : Request.Path.Value ?? "Unknown path";
        
        _logger.StatusCodeError(path, statusCode);

        ViewBag.StatusCode = statusCode;
        
        return statusCode switch
        {
            404 => View("NotFound"),
            _   => View("Index")
        };
    }
}