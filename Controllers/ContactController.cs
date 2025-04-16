using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;
using GreTutor.Models.Entities;

namespace GreTutor.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}


