using GreTutor.Models;
using GreTutor.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GreTutor.Data;
using System.Diagnostics;
using GreTutor.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClassroomController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager, ILogger<HomeController> logger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var classMembers = await _context.ClassMembers
                .Where(cm => cm.UserId == userId)
                .Include(cm => cm.Class)
                .ToListAsync();

            var classIds = classMembers.Select(cm => cm.ClassId).ToList();

            var documents = await _context.Documents
                .Where(d => classIds.Contains(d.ClassId))
                .Include(d => d.UploadedBy)
                .ToListAsync();

            var viewModel = new ClassroomViewModel
            {
                ClassMembers = classMembers,
                Documents = documents
            };

            return View(viewModel);
        }

        public async Task<IActionResult> People(int classId)
        {
            if (classId <= 0)
            {
                return NotFound();
            }

            var classMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == classId)
                .Include(cm => cm.User)
                .ToListAsync();

            ViewBag.ClassId = classId;

            var userRoles = new Dictionary<string, string>();
            foreach (var member in classMembers) // ✅ Sửa lại tên biến
            {
                var roles = await _userManager.GetRolesAsync(member.User);
                userRoles[member.UserId] = string.Join(", ", roles);
            }
            ViewBag.UserRoles = userRoles;

            return View(classMembers);
        }


        public async Task<IActionResult> Classwork(int classId)
        {
            var documents = await _context.Documents
                .Where(d => d.ClassId == classId)
                .Include(d => d.UploadedBy) // ⚠️ Bắt buộc Include để load dữ liệu User
                .ToListAsync();

            ViewBag.ClassId = classId;
            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(int classId, string FileName, IFormFile FileUpload)
        {
            try
            {
                _logger.LogInformation("📌 Bắt đầu xử lý upload file...");

                if (FileUpload == null || FileUpload.Length == 0)
                {
                    _logger.LogWarning("❌ Không có file nào được chọn để upload.");
                    ModelState.AddModelError("FileUpload", "Vui lòng chọn file để upload.");
                    return RedirectToAction("Classwork", new { classId = classId });
                }

                // Kiểm tra thư mục lưu file
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    _logger.LogInformation("📂 Tạo thư mục uploads...");
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FileUpload.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    _logger.LogInformation("📝 Đang lưu file: " + filePath);
                    await FileUpload.CopyToAsync(fileStream);
                }

                string relativePath = "/uploads/" + uniqueFileName;
                _logger.LogInformation("✅ File đã lưu: " + relativePath);

                // Lấy thông tin người dùng hiện tại
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogError("⚠️ Không tìm thấy user hiện tại.");
                    return Unauthorized();
                }

                // Tạo document mới
                Document document = new Document
                {
                    ClassId = classId,
                    FileName = FileName,
                    FilePath = relativePath,
                    UploadedAt = DateTime.UtcNow,
                    UploadedById = currentUser.Id
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
                _logger.LogInformation("✅ Document đã lưu vào DB: " + document.FileName);

                return RedirectToAction("Classwork", new { classId = classId });
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Upload error: " + ex.Message);
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var isStaff = User.IsInRole("Staff"); // Kiểm tra nếu user là Staff

            if (document.UploadedById != currentUser.Id && !isStaff)
            {
                return Forbid(); // Không cho xóa nếu không phải chủ sở hữu hoặc Staff
            }

            // Xóa file khỏi hệ thống
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, document.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            return RedirectToAction("Classwork", new { classId = document.ClassId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember(int classId, string memberId)
        {
            if (string.IsNullOrEmpty(memberId) || classId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid data.";
                return RedirectToAction("Index", new { classId });
            }

            var classMember = await _context.ClassMembers
                .FirstOrDefaultAsync(cm => cm.ClassId == classId && cm.UserId == memberId);

            if (classMember == null)
            {
                TempData["ErrorMessage"] = "No members found.";
                return RedirectToAction("Index", new { classId });
            }

            try
            {
                _context.ClassMembers.Remove(classMember);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Member removed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message; // Hiển thị lỗi nếu có
            }

            return RedirectToAction("People", new { classId });
        }

    }

}
