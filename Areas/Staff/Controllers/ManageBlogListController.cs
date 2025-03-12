using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GreTutor.Models;
using GreTutor.Data;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public class ManageBlogListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageBlogListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách tất cả BlogPost
        public async Task<IActionResult> Index()
        {
            // Sắp xếp theo thời gian tạo giảm dần (mới nhất ở đầu)
            var blogPosts = await _context.BlogPosts
            .Include(b => b.User)
            .OrderByDescending(b => b.Created)
            .ToListAsync();
            return View(blogPosts); 
        }

        // Action Approve: Đánh dấu BlogPost là Approved
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                blogPost.Status = BlogStatus.Approved;
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            // return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "ManageBlogList");
        }

        // Action Reject: Đánh dấu BlogPost là Rejected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                blogPost.Status = BlogStatus.Rejected;
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Action Delete: Xóa BlogPost khỏi cơ sở dữ liệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
