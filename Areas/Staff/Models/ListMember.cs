using GreTutor.Areas.Staff.Models;
using GreTutor.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GreTutor.Areas.Staff.Models
{
    public class ListMemberModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();  // ✅ Khởi tạo danh sách rỗng
        [TempData]
        public string StatusMessage { get; set; } = ""; // ✅ Khởi tạo chuỗi rỗng

        public ListMemberModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public class UserAndRole : IdentityUser
        {
            public string RoleNames { get; set; } = ""; // ✅ Khởi tạo chuỗi rỗng
        }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]

        public int currentPage { get; set; }

        public int countPages { get; set; }

        public int totalUsers { get; set; }


        public List<UserAndRole> users { set; get; }

        // [TempData] // Sử dụng Session lưu thông báo
        // public string StatusMessage { get; set; } = "";

        public async Task OnGet()
        {
            var qr = _userManager.Users.OrderBy(u => u.UserName);
            totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);
            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPages)
                currentPage = countPages;
            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Select(u => new UserAndRole()
                {
                    Id = u.Id,
                    UserName = u.UserName,
                });
            users = await qr1.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }

}

