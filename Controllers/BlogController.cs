using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreTutor.Data;
using GreTutor.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;



namespace GreTutor.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User); // Lấy ID của user hiện tại

            var blogPosts = await _context.BlogPosts
                .Where(b => b.AuthorId == userId) // Chỉ lấy bài viết của user hiện tại
                .Include(b => b.User) // Lấy thông tin User từ bảng Users
                .ToListAsync();

            return View(blogPosts);
        }

        public async Task<IActionResult> HomeBlog()
        {
            var approvedBlogs = await _context.BlogPosts
                .Where(b => b.Status == BlogStatus.Approved)
                .Include(b => b.User) // Lấy thông tin tác giả
                .OrderByDescending(b => b.Created)
                .ToListAsync();

            ViewBag.CurrentUser = User.Identity.Name; // Lưu user đang đăng nhập

            return View(approvedBlogs);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var blogPost = await _context.BlogPosts
                .Include(b => b.Comments)
                .ThenInclude(c => c.User!) 
                .FirstOrDefaultAsync(m => m.BlogId == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }


        public IActionResult Create()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newPost = new BlogPost
            {
                AuthorId = currentUserId
            };

            return View(newPost);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Created")] BlogPost blogPost)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(); // Nếu chưa đăng nhập, trả về lỗi 401
            }

            // ✅ Lấy ID của user hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            blogPost.AuthorId = userId;
            blogPost.Created = DateTime.Now;
            blogPost.Status = BlogStatus.Pending;

            // 🔥 Reset ModelState trước khi kiểm tra validation
            ModelState.Clear();
            TryValidateModel(blogPost);

            // 🛠 Debug: Kiểm tra dữ liệu đầu vào
            Console.WriteLine($"✅ User ID: {userId}");
            Console.WriteLine($"📝 Title: {blogPost.Title}");
            Console.WriteLine($"📖 Content: {blogPost.Content}");
            Console.WriteLine($"⏳ Created: {blogPost.Created}");
            Console.WriteLine($"👤 AuthorId: {blogPost.AuthorId}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is INVALID!");
                foreach (var error in ModelState)
                {
                    foreach (var err in error.Value.Errors)
                    {
                        Console.WriteLine($"⚠ Validation Error: {error.Key} - {err.ErrorMessage}");
                    }
                }
                return View(blogPost);
            }

            try
            {
                _context.Add(blogPost);
                await _context.SaveChangesAsync();
                Console.WriteLine("✅ Blog post saved successfully!");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🔥 Error saving to database: {ex.Message}");
                return View(blogPost);
            }
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            return View(blogPost);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,Title,Content,Created")] BlogPost blogPost)
        {
            if (id != blogPost.BlogId)
            {
                return NotFound();
            }

            // ✅ Lấy ID user hiện tại
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            blogPost.AuthorId = userId;
            blogPost.Created = DateTime.Now;
            blogPost.Status = BlogStatus.Pending;

            // ✅ Xóa lỗi AuthorId khỏi ModelState
            ModelState.Remove("AuthorId");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(blogPost).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.BlogId))
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

            return View(blogPost);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Console.WriteLine($"==== DEBUG: Nhận yêu cầu xóa BlogPost ID: {id} ====");
            try
            {
                if (_context.BlogPosts == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.BlogPosts' is null.");
                }

                // 🔍 Debug: Log ID được nhận
                Console.WriteLine($"==== DEBUG: Nhận yêu cầu xóa BlogPost ID: {id} ====");

                var blogPost = await _context.BlogPosts.FindAsync(id);

                if (blogPost == null)
                {
                    Console.WriteLine("==== DEBUG: Không tìm thấy BlogPost! ====");
                    return NotFound();
                }

                // 🔍 Debug: Log thông tin bài viết trước khi xóa
                Console.WriteLine($"==== DEBUG: BlogPost trước khi xóa ====");
                Console.WriteLine($"BlogId: {blogPost.BlogId}");
                Console.WriteLine($"Title: {blogPost.Title}");
                Console.WriteLine($"Content: {blogPost.Content}");
                Console.WriteLine($"AuthorId: {blogPost.AuthorId}");
                Console.WriteLine($"Created: {blogPost.Created}");
                Console.WriteLine($"Status: {blogPost.Status}");
                Console.WriteLine($"======================================");

                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();

                // 🔍 Debug: Log sau khi xóa thành công
                Console.WriteLine($"==== DEBUG: Đã xóa BlogPost ID: {id} ====");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 🔍 Debug: Log lỗi nếu có
                Console.WriteLine($"==== ERROR: Lỗi khi xóa BlogPost ID: {id} ====");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                return Problem("Đã xảy ra lỗi trong quá trình xóa bài viết.");
            }
        }


        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.BlogId == id)).GetValueOrDefault();
        }
    }
}
