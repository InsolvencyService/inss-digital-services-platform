using INSS.Platform.AlphaDemo.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.AlphaDemo.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ContactUs()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ContactUs(ContactUsViewModel model)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("ContactUsCompleted", new { name = model.Name });
        }

        return View(model);
    }

    public IActionResult ContactUsCompleted(string name)
    {
        return View(model: name);
    }

    public IActionResult ReportIssue()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ReportIssue(ReportIssueViewModel model)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction("ReportIssueCompleted", new { name = model.Name });
        }

        return View(model);
    }

    public IActionResult ReportIssueCompleted(string name)
    {
        return View(model: name); 
    }

    public IActionResult SiteMap()
    {
        return View();
    }
}
