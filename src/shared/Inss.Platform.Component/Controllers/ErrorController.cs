using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoggerExtensions = Inss.Platform.Component.Extensions.LoggerExtensions;

namespace Inss.Platform.Component.Controllers;

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
            LoggerExtensions.UnexpectedError(_logger, path, message);
        }
        
        return View();
    }
    
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatus(int statusCode)
    {
        LoggerExtensions.StatusCodeError(_logger, Request.Path, statusCode);

        ViewBag.StatusCode = statusCode;
        
        return statusCode switch
        {
            404 => View("NotFound"),
            _   => View("Index")
        };
    }
}