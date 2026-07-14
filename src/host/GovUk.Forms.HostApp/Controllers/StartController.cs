using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.HostApp.Controllers;

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