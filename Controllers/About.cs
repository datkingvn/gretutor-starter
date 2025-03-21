using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;

namespace GreTutor.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}


