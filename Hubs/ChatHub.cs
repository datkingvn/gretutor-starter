using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using GreTutor.Data;
using GreTutor.Models.Entities;

namespace GreTutor.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ChatHub(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task JoinClassChat(int classId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"class-{classId}");
        }

        public async Task SendMessage(int classId, string message)
        {
            var userId = Context.UserIdentifier;
            var sender = await _userManager.FindByIdAsync(userId);

            if (sender == null)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", new
                {
                    SenderName = "Hệ thống",
                    Message = "Lỗi: Không thể gửi tin nhắn"
                });
                return;
            }

            var vietnamTime = DateTime.UtcNow.AddHours(7);

            var chatMessage = new ChatMessage
            {
                ClassId = classId,
                SenderId = userId,
                Message = message,
                SentAt = vietnamTime
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Gửi tin nhắn đến tất cả user trong nhóm
            await Clients.Group($"class-{classId}").SendAsync("ReceiveMessage", new
            {
                SenderId = sender.Id,
                SenderName = sender.UserName,
                Message = chatMessage.Message,
                SentAt = vietnamTime.ToString("HH:mm")
            });
        }
    }

}
