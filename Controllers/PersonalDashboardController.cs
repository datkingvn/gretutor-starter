using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;
using GreTutor.Models.Entities;
using GreTutor.Data;
using System.Security.Claims;
using GreTutor.Models.Enums;
using GreTutor.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;


namespace GreTutor.Controllers
{
    [Authorize]
    public class PersonalDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Tên constructor phải trùng với tên class:
        public PersonalDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy userId hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Đếm blog theo status
            var pending = await _context.BlogPosts
                .CountAsync(b => b.AuthorId == userId && b.Status == BlogStatus.Pending);
            var approved = await _context.BlogPosts
                .CountAsync(b => b.AuthorId == userId && b.Status == BlogStatus.Approved);
            var rejected = await _context.BlogPosts
                .CountAsync(b => b.AuthorId == userId && b.Status == BlogStatus.Rejected);

            // Lấy danh sách classId mà user đang tham gia
            var classIds = await _context.ClassMembers
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.ClassId)
                .ToListAsync();

            // Đếm meetings và documents trong các class đó
            var meetings = await _context.Meetings
                .CountAsync(m => classIds.Contains(m.ClassId));
            var documents = await _context.Documents
                .CountAsync(d => classIds.Contains(d.ClassId));

            var vm = new PersonalDashboardViewModel
            {
                PendingBlogPosts = pending,
                ApprovedBlogPosts = approved,
                RejectedBlogPosts = rejected,
                TotalMeetings = meetings,
                TotalDocuments = documents
            };

            return View(vm);
        }
    }
}


