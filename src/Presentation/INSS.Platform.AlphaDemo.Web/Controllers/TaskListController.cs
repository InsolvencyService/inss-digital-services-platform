using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

[Authorize]
public class TaskListController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
