using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace GreTutor.Areas.Staff.Models
{
    public class AddRoleModel
    {
        public IdentityUser User { get; set; } // Đảm bảo User không null

        [DisplayName("Roles assigned to users")]
        public string[] RoleNames { get; set; } = new string[] {}; // Tránh null exception

        public SelectList AllRoles { get; set; } = new SelectList(Enumerable.Empty<string>()); // Tránh null exception

        public string StatusMessage { get; set; }
    }
}
