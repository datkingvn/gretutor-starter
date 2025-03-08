using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;

namespace GreTutor.Controllers;

public class ClassroomController : Controller
{
    public IActionResult Overview()
    {     
        return View();
    }
    public IActionResult Students()
    {
        return View();
    }
    public IActionResult Engagement()
    {
        return View();
    }
    public IActionResult Reviews()
    {
        return View();
    }
    public IActionResult TrafficConversion()
    {
        return View();
    }
    
}
