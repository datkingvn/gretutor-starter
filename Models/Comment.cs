using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 🔹 Khóa ngoại liên kết với BlogPost
        [Required]
        public int BlogPostId { get; set; }

        [ForeignKey("BlogPostId")]
        public BlogPost BlogPost { get; set; }

        // 🔹 Khóa ngoại liên kết với User (người bình luận)
        [Required]
        public string UserId { get; set; }

        public IdentityUser User { get; set; } // Không cần [ForeignKey] ở đây
    }
}


