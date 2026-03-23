using GovUk.Forms.Components;
using GovUk.Forms.HostApp.Model;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Forms.HostApp.Controllers;

public class HomeController : Controller
{
    private readonly IEnumerable<IWebRoot> _webRoots;

    public HomeController(IEnumerable<IWebRoot> webRoots)
    {
        _webRoots = webRoots;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        HomeModel home = HomeModel.Create(_webRoots);
        return View(home);
    }
}