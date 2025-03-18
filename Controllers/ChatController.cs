using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GreTutor.Data;
using GreTutor.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GreTutor.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace GreTutor.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Hiển thị danh sách tin nhắn của lớp
        public async Task<IActionResult> Index(int classId)
        {
            var userId = _userManager.GetUserId(User);
            var messages = await _context.ChatMessages
                .Where(m => m.ClassId == classId)
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .ToListAsync();

            ViewBag.ClassId = classId;
            ViewBag.CurrentUserId = userId;

            return View(messages);
        }




        // Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> SendMessage(int classId, string messageContent)
        {
            if (string.IsNullOrEmpty(messageContent))
            {
                ModelState.AddModelError("", "Nội dung tin nhắn không được để trống.");
                return RedirectToAction("Index", new { classId });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var chatMessage = new ChatMessage
            {
                ClassId = classId,
                SenderId = user.Id,
                Message = messageContent,
                SentAt = DateTime.UtcNow
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { classId });
        }
    }
}
