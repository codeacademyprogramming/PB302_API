using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace UniversityApp.UI.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string? message)
        {
            TempData["Error"] = message??"Something went wrong!";
            return View();
        }


    }
}
