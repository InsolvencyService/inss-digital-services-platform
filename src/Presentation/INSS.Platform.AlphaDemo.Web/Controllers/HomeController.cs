using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

/// <summary>
/// Provides endpoints for rendering the Home page.
/// </summary>
public class HomeController : Controller
{
    /// <summary>
    /// Returns the default Home view.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the Home page view.
    /// </returns>
    public IActionResult Index()
    {
        return View();
    }
}
