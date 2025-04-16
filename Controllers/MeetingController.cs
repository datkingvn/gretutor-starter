using GreTutor.Data;
using GreTutor.Models.Entities;
using GreTutor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace GreTutor.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ZoomService _zoomService;
        private readonly IEmailSender _emailSender;

        public MeetingController(ApplicationDbContext context, ZoomService zoomService, IEmailSender emailSender)
        {
            _context = context;
            _zoomService = zoomService;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Staff,Tutor")]
        public IActionResult Create(int classId)
        {
            if (classId == 0)
            {
                return RedirectToAction("Index", "Class");
            }

            var classEntity = _context.Classes.Find(classId);
            if (classEntity == null)
            {
                return NotFound();
            }

            var meeting = new Meeting
            {
                ClassId = classId,
                Class = classEntity,
                StartTime = DateTime.Now
            };

            return View(meeting);
        }

        [Authorize(Roles = "Staff,Tutor")]
        [HttpPost]
        public async Task<IActionResult> Create(Meeting meeting)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Nhận cuộc họp: {JsonConvert.SerializeObject(meeting)}");

                // Kiểm tra ClassId hợp lệ
                if (meeting.ClassId == 0)
                {
                    ModelState.AddModelError("", "ClassId không hợp lệ.");
                    Console.WriteLine("[DEBUG] ClassId không hợp lệ");
                    return View(meeting);
                }

                // Kiểm tra ModelState
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("[DEBUG] ModelState không hợp lệ:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Lỗi: {error.ErrorMessage}");
                    }

                    TempData["Error"] = string.Join("<br>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return View(meeting);
                }

                // Nếu là họp Online -> tạo Zoom Link
                if (meeting.Location == "Online")
                {
                    try
                    {
                        meeting.MeetingLink = await _zoomService.CreateMeeting(meeting);
                        Console.WriteLine($"[DEBUG] Link cuộc họp Zoom: {meeting.MeetingLink}");

                        if (string.IsNullOrEmpty(meeting.MeetingLink))
                        {
                            Console.WriteLine("[ERROR] Zoom API không trả về link họp");
                            ModelState.AddModelError("", "Không thể tạo cuộc họp Zoom.");
                            return View(meeting);
                        }

                        // 🔹 Thêm link Google Drive mặc định cho Recording
                        meeting.RecordingLink = "https://drive.google.com/drive/folders/1O-DOOziPi7tzHbn6H0Xnfi3J4N-hAQBf?usp=sharing";
                        Console.WriteLine($"[DEBUG] Link bản ghi mặc định: {meeting.RecordingLink}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Lỗi tạo cuộc họp Zoom: {ex.Message}");
                        ModelState.AddModelError("", $"Lỗi khi tạo link họp: {ex.Message}");
                        return View(meeting);
                    }
                }

                // Lưu vào database
                _context.Meetings.Add(meeting);
                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] Cuộc họp đã lưu vào database: ID = {meeting.Id}");

                // Gửi thông báo đến thành viên lớp học
                await NotifyClassMembers(meeting);

                return RedirectToAction("Index", new { classId = meeting.ClassId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu cuộc họp: {ex.Message}");
                Console.WriteLine($"[DEBUG] Nhận cuộc họp: {JsonConvert.SerializeObject(meeting)}");
                ModelState.AddModelError("", $"Lỗi khi lưu cuộc họp: {ex.Message}");
                return View(meeting);
            }
        }



        public async Task<IActionResult> Index(int classId)
        {
            if (classId == 0)
            {
                return RedirectToAction("Index", "Class");
            }

            ViewBag.ClassId = classId; // Gán ClassId để dùng trong View
            var meetings = await _context.Meetings.Where(m => m.ClassId == classId).ToListAsync();
            return View(meetings);
        }


        // 🔹 Gửi email cho tất cả thành viên lớp
        private async Task NotifyClassMembers(Meeting meeting)
        {
            var classMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == meeting.ClassId)
                .Include(cm => cm.User)
                .ToListAsync();

            foreach (var member in classMembers)
            {
                string subject = $"[Announcement] New meeting: {meeting.Title}";
                string body = $@"
                    <p>Hello {member.User.UserName},</p>
                    <p>A new meeting has been scheduled:</p>
                    <ul>
                        <li><strong>Title:</strong> {meeting.Title}</li>
                        <li><strong>Time:</strong> {meeting.StartTime}</li>
                        <li><strong>Location:</strong> {meeting.Location}</li>
                        <li><strong>Meeting Link:</strong> <a href='{meeting.MeetingLink}'>{meeting.MeetingLink}</a></li>
                        <li><strong>Notes:</strong> {meeting.Note}</li>
                    </ul>
                    <p>Please join on time.</p>
                ";

                await _emailSender.SendEmailAsync(member.User.Email, subject, body);
            }
        }

        [Authorize(Roles = "Staff,Tutor")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);

            if (meeting == null)
            {
                return NotFound();
            }

            // Lấy danh sách thành viên lớp
            var classMembers = await _context.ClassMembers
                .Where(cm => cm.ClassId == meeting.ClassId)
                .Include(cm => cm.User)
                .ToListAsync();

            // Xóa cuộc họp
            _context.Meetings.Remove(meeting);
            await _context.SaveChangesAsync();

            // Gửi email thông báo xóa cuộc họp
            await NotifyClassMembersMeetingDeleted(meeting, classMembers);

            TempData["Success"] = "The meeting has been deleted and the notification email has been sent!";
            return RedirectToAction("Index", new { classId = meeting.ClassId });
        }

        private async Task NotifyClassMembersMeetingDeleted(Meeting meeting, List<ClassMember> classMembers)
        {
            foreach (var member in classMembers)
            {
                string subject = $"[Notification] Meeting Canceled: {meeting.Title}";
                string body = $@"
            <p>Hello {member.User.UserName},</p>
            <p>The following meeting has been <strong>canceled</strong>:</p>
            <ul>
                <li><strong>Title:</strong> {meeting.Title}</li>
                <li><strong>Original Time:</strong> {meeting.StartTime}</li>
                <li><strong>Location:</strong> {meeting.Location}</li>
                <li><strong>Notes:</strong> {meeting.Note}</li>
            </ul>
            <p>Sorry for the inconvenience.</p>
        ";

                await _emailSender.SendEmailAsync(member.User.Email, subject, body);
            }
        }

    }
}
