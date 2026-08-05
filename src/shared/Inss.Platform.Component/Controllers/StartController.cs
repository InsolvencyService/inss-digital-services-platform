using Inss.Platform.Component.Resolvers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.Platform.Component.Controllers;

public class StartController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        IStartPageResolver startPageResolver = HttpContext.RequestServices.GetService<IStartPageResolver>() ?? new DefaultStartPageResolver();
        IActionResult result = startPageResolver.Resolve();
        return result;
    }
}