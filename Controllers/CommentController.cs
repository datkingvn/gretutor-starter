using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GreTutor.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GreTutor.Data;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(int blogId)
        {
            if (blogId <= 0) return BadRequest("BlogId không hợp lệ.");

            var comments = await _context.Comments
                .Where(c => c.BlogId == blogId)
                .Include(c => c.User!) // Nếu `User` bị null, kiểm tra FK
                .OrderByDescending(c => c.Created)
                .ToListAsync();

            return View(comments);
        }


        [HttpPost]
        public async Task<IActionResult> Create(int blogId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest("Nội dung bình luận không được để trống.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                Content = content,
                Created = DateTime.Now,
                AuthorId = user.Id,
                BlogId = blogId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(); // 🔥 Lưu vào database

            return RedirectToAction("Details", "Blog", new { id = blogId });
        }


        [HttpPost]
        public IActionResult Delete(int commentId, int blogId)
        {
            var comment = _context.Comments
                .Include(c => c.User) // Lấy thông tin người đăng
                .FirstOrDefault(c => c.CommentId == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            // Chỉ cho phép xóa nếu là tác giả hoặc admin
            if (User.Identity.Name == comment.User?.UserName || User.IsInRole("Admin"))
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Blog", new { id = blogId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int CommentId, string Content)
        {
            var comment = await _context.Comments.FindAsync(CommentId);
            if (comment == null)
            {
                return NotFound();
            }

            // Lấy ID của user đang đăng nhập
            var currentUserId = _userManager.GetUserId(User);

            // Kiểm tra quyền chỉnh sửa: chỉ cho phép người tạo hoặc admin chỉnh sửa
            if (comment.AuthorId != currentUserId && !User.IsInRole("Admin"))
            {
                return Forbid(); // Trả về lỗi 403 nếu không có quyền
            }

            // Cập nhật nội dung bình luận
            comment.Content = Content;
            comment.Created = DateTime.Now; // Có thể cập nhật thời gian chỉnh sửa nếu cần

            await _context.SaveChangesAsync();

            // Chuyển hướng về trang chi tiết bài viết chứa bình luận
            return RedirectToAction("Details", "Blog", new { id = comment.BlogId });
        }



    }
}
