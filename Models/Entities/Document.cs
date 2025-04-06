using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace GreTutor.Models.Entities
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [Required]
        public string UploadedById { get; set; }
        [ForeignKey("UploadedById")]
        public virtual IdentityUser UploadedBy { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string FileName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string FilePath { get; set; } // Đường dẫn file

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<CommentDocument> CommentDocuments { get; set; } = new List<CommentDocument>();
    }

}