using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GreTutor.Data;
using GreTutor.Models.Entities;
using GreTutor.Models.ViewModels;
using GreTutor.Areas.Staff.Models;
using GreTutor.Areas.Staff.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;


namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class ClassMemberController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSender _emailSender;

        private const int ITEMS_PER_PAGE = 10;

        public ClassMemberController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }


        public async Task<IActionResult> Index(int classId)
        {
            if (classId <= 0)
            {
                return NotFound();
            }

            var classMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == classId)
                .Include(cm => cm.User)
                .ToListAsync();

            if (classMembers == null || classMembers.Count == 0)
            {
                ViewBag.Message = "There are no members in the class.";
            }

            ViewBag.ClassId = classId;
            return View(classMembers);
        }

        public async Task<IActionResult> Add(int classId)
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            // Lấy danh sách UserId đã thuộc bất kỳ lớp nào
            var usersInClasses = await _context.ClassMembers
                .Select(cm => cm.UserId)
                .Distinct()
                .ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.Any() ? string.Join(", ", roles) : "No Role";

                // 🚀 Kiểm tra nếu user đã thuộc bất kỳ lớp nào thì bỏ qua
                if (!usersInClasses.Contains(user.Id))
                {
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleNames = userRole
                    });
                }
            }

            var model = new AddClassMemberViewModel
            {
                ClassId = classId,
                Users = userViewModels
            };

            return View(model);
        }


        // Xử lý khi nhấn nút Add
        // [HttpPost]
        // public async Task<IActionResult> AddMember(int classId, string userId)
        // {
        //     if (string.IsNullOrEmpty(userId) || classId <= 0)
        //     {
        //         TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
        //         return RedirectToAction("Index", new { classId });
        //     }

        //     var existingMember = await _context.ClassMembers
        //         .FirstOrDefaultAsync(cm => cm.ClassId == classId && cm.UserId == userId);

        //     if (existingMember != null)
        //     {
        //         TempData["ErrorMessage"] = "Thành viên này đã có trong lớp.";
        //         return RedirectToAction("Index", new { classId });
        //     }

        //     // 🔥 Lấy Role của user từ Identity
        //     var user = await _userManager.FindByIdAsync(userId);
        //     var roles = await _userManager.GetRolesAsync(user);
        //     string userRole = roles.FirstOrDefault() ?? "Student"; // Nếu user không có role, gán mặc định "Student"

        //     // 🌟 Thêm user vào lớp với role lấy từ Identity
        //     var newMember = new ClassMember
        //     {
        //         ClassId = classId,
        //         UserId = userId,
        //         Role = userRole
        //     };

        //     _context.ClassMembers.Add(newMember);
        //     await _context.SaveChangesAsync();

        //     TempData["SuccessMessage"] = "Thành viên đã được thêm thành công!";
        //     return RedirectToAction("Index", new { classId });
        // }

        [HttpPost]
        public async Task<IActionResult> AddMultipleMembers(int classId, List<string> selectedUsers, [FromServices] IEmailSender emailSender)
        {
            if (selectedUsers == null || !selectedUsers.Any() || classId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return RedirectToAction("Index", new { classId });
            }

            var existingMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == classId && selectedUsers.Contains(cm.UserId))
                .Select(cm => cm.UserId)
                .ToListAsync();

            var newMembers = new List<ClassMember>();

            foreach (var userId in selectedUsers)
            {
                if (existingMembers.Contains(userId)) continue;

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) continue;

                var roles = await _userManager.GetRolesAsync(user);
                string userRole = roles.FirstOrDefault() ?? "Student";

                newMembers.Add(new ClassMember
                {
                    ClassId = classId,
                    UserId = userId,
                    Role = userRole
                });

                // Gửi email thông báo
                string subject = "You have been added to a new class!";
                string message = $"Hello {user.UserName},<br><br>"
                               + $"You have been successfully added to class ID: {classId}.<br>"
                               + "Please check your account for more details.<br><br>"
                               + "Best regards,<br>eTutoring Team";

                await emailSender.SendEmailAsync(user.Email, subject, message);
            }

            if (newMembers.Any())
            {
                _context.ClassMembers.AddRange(newMembers);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{newMembers.Count} Member(s) added and notified!";
            }
            else
            {
                TempData["ErrorMessage"] = "No members added.";
            }

            return RedirectToAction("Add", new { classId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int classId, string userId)
        {
            if (string.IsNullOrEmpty(userId) || classId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return RedirectToAction("Add", new { classId }); // 🔄 Chuyển về Add để hiển thị lại danh sách
            }

            var classMember = await _context.ClassMembers
                .FirstOrDefaultAsync(cm => cm.ClassId == classId && cm.UserId == userId);

            if (classMember == null)
            {
                TempData["ErrorMessage"] = "No member found in this class.";
                return RedirectToAction("Add", new { classId }); // 🔄 Cũng redirect về Add
            }

            // 🔹 Xóa thành viên khỏi lớp
            _context.ClassMembers.Remove(classMember);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Member removed successfully!";

            return RedirectToAction("Index", new { classId }); // 🚀 Chuyển về trang Add
        }

    }
}
