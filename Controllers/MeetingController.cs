using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreTutor.Data;
using GreTutor.Models.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeetingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách cuộc họp của lớp
        public async Task<IActionResult> Index(int classId)
        {
            var classExists = await _context.Classes.AnyAsync(c => c.Id == classId);
            if (!classExists)
            {
                return NotFound("Class not found.");
            }

            var meetings = await _context.Meetings
                .Where(m => m.ClassId == classId)
                .OrderBy(m => m.StartTime)
                .ToListAsync();

            ViewBag.ClassId = classId;
            return View(meetings);
        }

        // Hiển thị form tạo cuộc họp
        [HttpGet]
        public IActionResult Create(int classId)
        {
            return View(new Meeting { ClassId = classId });
        }

        // Xử lý tạo cuộc họp
        [HttpPost]
        public async Task<IActionResult> Create(Meeting meeting)
        {
            if (!ModelState.IsValid)
            {
                return View(meeting);
            }

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { classId = meeting.ClassId });
        }

        // Hiển thị form chỉnh sửa cuộc họp
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }
            return View(meeting);
        }

        // Xử lý cập nhật cuộc họp
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Meeting meeting)
        {
            if (id != meeting.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(meeting);
            }

            _context.Update(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { classId = meeting.ClassId });
        }

        // Xóa cuộc họp
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return NotFound();
            }

            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { classId = meeting.ClassId });
        }
    }
}
