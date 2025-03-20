using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } 
        public string UserName { get; set; } 
        public string Email { get; set; } 
        public string RoleNames { get; set; } 
    }
}
