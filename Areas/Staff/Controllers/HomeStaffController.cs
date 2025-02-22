using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;

namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    public class HomeStaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}


