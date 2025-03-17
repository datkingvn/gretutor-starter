using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } // ID của User (IdentityUser.Id)
        public string UserName { get; set; } // Tên tài khoản
        public string Email { get; set; } // Gmail (Email)
        public string RoleNames { get; set; } // Danh sách Role của User (chuỗi phân cách bằng dấu phẩy nếu nhiều role)
    }
}
