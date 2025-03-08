using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        public string AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual IdentityUser? User { get; set; }

        // Khóa ngoại liên kết với BlogPost
        public int BlogId { get; set; }
        [ForeignKey("BlogId")]
        public virtual BlogPost? BlogPost { get; set; }
    }
}
