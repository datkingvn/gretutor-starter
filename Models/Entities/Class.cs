using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace GreTutor.Models.Entities
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }

        // Danh sách thành viên trong lớp
        public virtual List<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();

        // Danh sách tài liệu
        public virtual List<Document> Documents { get; set; } = new List<Document>();

        // Danh sách cuộc họp
        public virtual List<Meeting> Meetings { get; set; } = new List<Meeting>();

        // Danh sách tin nhắn trong lớp
        public virtual List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}

