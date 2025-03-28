using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace GreTutor.Models.Entities
{
    public class CommentDocument
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
        public virtual IdentityUser User { get; set; }

        public int DocumentId { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Document? Document { get; set; }
    }

}
