using Microsoft.AspNetCore.Mvc;
using GreTutor.Models;
using GreTutor.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GreTutor.Data;
using GreTutor.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DocumentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int documentId)
        {
            // Get the document including its comments
            var document = _context.Documents
                .Include(d => d.CommentDocuments)  // Include the related comments
                .FirstOrDefault(d => d.Id == documentId);

            if (document == null)
            {
                return NotFound();
            }

            // Create a DocumentViewModel and assign the values
            var model = new DocumentViewModel
            {
                DocumentId = document.Id,
                Title = document.FileName,
                CommentDocuments = document.CommentDocuments
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var document = _context.Documents
                .Include(d => d.CommentDocuments)
                .ThenInclude(cd => cd.User)  // Nếu bạn cần thông tin người dùng cho comment
                .FirstOrDefault(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            // Tạo DocumentViewModel từ dữ liệu Document
            var viewModel = new DocumentViewModel
            {
                DocumentId = document.Id,
                Title = document.FileName,  // Giả sử bạn muốn hiển thị tên file
                ClassId = document.ClassId,
                CommentDocuments = document.CommentDocuments.ToList()  // Lấy tất cả comment của tài liệu
            };

            return View(viewModel);
        }



        // Xử lý thêm bình luận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CommentDocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var comment = new CommentDocument
                {
                    Content = model.Content,
                    Created = DateTime.Now,
                    AuthorId = user.Id,
                    DocumentId = model.DocumentId  // Gán DocumentId cho CommentDocument
                };

                _context.CommentDocuments.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = model.DocumentId }); // Truyền DocumentId vào
            }

            return View(model);
        }

        public IActionResult EditComment(int id)
        {
            // Fetch the comment document along with the associated Document using eager loading
            var commentDocument = _context.CommentDocuments
                                          .Include(c => c.Document)  // Ensure the Document is loaded
                                          .FirstOrDefault(c => c.CommentId == id);

            if (commentDocument == null)
            {
                return NotFound();  // If the comment document isn't found, return 404
            }

            // Check if the user has the right to edit the comment
            if (User.Identity.Name != commentDocument.AuthorId && !User.IsInRole("Staff"))
            {
                return Unauthorized();  // Only the author or staff can edit
            }

            // Return the view with the comment for editing (you can pass the entire CommentDocument model to the view)
            return View(commentDocument);
        }

        // POST action to update the comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int CommentId, string Content)
        {
            var comment = await _context.CommentDocuments.FindAsync(CommentId);
            if (comment == null)
            {
                return NotFound(); // Nếu không tìm thấy bình luận
            }

            // Kiểm tra quyền của người dùng
            var currentUserId = _userManager.GetUserId(User);
            if (comment.AuthorId != currentUserId && !User.IsInRole("Staff"))
            {
                return Forbid(); // Trả về lỗi 403 nếu không có quyền
            }

            // Cập nhật nội dung bình luận
            comment.Content = Content;
            comment.Created = DateTime.Now; // Cập nhật thời gian chỉnh sửa

            await _context.SaveChangesAsync();

            // Chuyển hướng về trang chi tiết tài liệu sau khi lưu
            return RedirectToAction("Details", "Document", new { id = comment.DocumentId });
        }




        // POST: Delete comment
        // DELETE action to delete a comment
        public IActionResult DeleteComment(int id)
        {
            var commentDocument = _context.CommentDocuments
                                          .Include(c => c.Document)
                                          .FirstOrDefault(c => c.CommentId == id);

            if (commentDocument == null)
            {
                return NotFound();
            }

            // Lấy User ID từ Claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var authorId = commentDocument.AuthorId.ToString();

            Console.WriteLine($"Current User ID: {userId}, Author ID: {authorId}");

            if (authorId != userId && !User.IsInRole("Staff"))
            {
                Console.WriteLine("Access Denied: User is neither Author nor Staff");
                return Forbid();
            }

            _context.CommentDocuments.Remove(commentDocument);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Comment deleted successfully.";

            return RedirectToAction("Details", "Document", new { id = commentDocument.DocumentId });
        }

    }
}
