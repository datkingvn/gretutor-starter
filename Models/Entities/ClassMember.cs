using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace GreTutor.Models.Entities
{
    public class ClassMember
    {
        public int Id { get; set; }

        // Khóa ngoại đến bảng Class
        public int ClassId { get; set; }
        public Class Class { get; set; }

        // Liên kết với IdentityUser
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        // Vai trò trong lớp: "Student" hoặc "Tutor"
        public string Role { get; set; }
    }
}

