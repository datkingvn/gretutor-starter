using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GreTutor.Data;
using GreTutor.Models.Entities;

namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class ClassController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClassController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Class
        public async Task<IActionResult> Index()
        {
            return _context.Classes != null ?
            View(await _context.Classes.ToListAsync()) :
            Problem("Entity set 'ApplicationDbContext.Classes'  is null.");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int classId)  // Đúng với tham số classId
        {
            if (classId <= 0)
            {
                return NotFound();
            }

            var classMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == classId)
                .Include(cm => cm.User)
                .ToListAsync();

            if (!classMembers.Any())
            {
                ViewBag.Message = "There are no members in the class.";
            }

            ViewBag.ClassId = classId;
            return View(classMembers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@class);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassExists(@class.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Classes == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            return View(@class);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Classes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Classes'  is null.");
            }
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

/*************  ✨ Windsurf Command ⭐  *************/
/*******  76d9413c-5797-4d16-81cf-b15d71cf115a  *******/
        private bool ClassExists(int id)
        {
            return (_context.Classes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
