using Microsoft.AspNetCore.Identity;

namespace GreTutor.Areas.Staff.Models
{
    public class UserAndRole : IdentityUser
    {
        public string RoleNames { get; set; }
    }
}
