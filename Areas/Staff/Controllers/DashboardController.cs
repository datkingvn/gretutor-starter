using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreTutor.Models.Enums;
using GreTutor.Data;
using Microsoft.AspNetCore.Authorization;
using GreTutor.Models.Entities;
using Microsoft.AspNetCore.Identity;
using GreTutor.Areas.Staff.Models.ViewModels;

namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context,
                                   UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> StaffDashboard()
        {
            var totalAccounts = await _context.Users.CountAsync();
            var totalBlogPosts = await _context.BlogPosts.CountAsync();
            var totalClasses = await _context.Classes.CountAsync();

            var roleCounts = await (from ur in _context.UserRoles
                                    join r in _context.Roles on ur.RoleId equals r.Id
                                    group ur by r.Name into g
                                    select new
                                    {
                                        RoleName = g.Key,
                                        Count = g.Count()
                                    }).ToListAsync();

            int totalStaffs = roleCounts.FirstOrDefault(x => x.RoleName == "Staff")?.Count ?? 0;
            int totalTutors = roleCounts.FirstOrDefault(x => x.RoleName == "Tutor")?.Count ?? 0;
            int totalStudents = roleCounts.FirstOrDefault(x => x.RoleName == "Student")?.Count ?? 0;

            int approvedBlogs = await _context.BlogPosts
                .CountAsync(b => b.Status == BlogStatus.Approved);

            int pendingBlogs = await _context.BlogPosts
                .CountAsync(b => b.Status == BlogStatus.Pending);

            int rejectedBlogs = await _context.BlogPosts
                .CountAsync(b => b.Status == BlogStatus.Rejected); // 👈 Thêm dòng này

            var viewModel = new DashboardViewModel
            {
                TotalAccounts = totalAccounts,
                TotalBlogPosts = totalBlogPosts,
                TotalClasses = totalClasses,
                TotalStaffs = totalStaffs,
                TotalTutors = totalTutors,
                TotalStudents = totalStudents,
                ApprovedBlogPosts = approvedBlogs,
                PendingBlogPosts = pendingBlogs,
                RejectedBlogPosts = rejectedBlogs // 👈 Gán thêm
            };

            return View(viewModel);
        }

    }

}

