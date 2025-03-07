using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
namespace GreTutor.Models
{
    public class BlogPost
    {
        [Key]
        public int BlogId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")] 
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        public string AuthorId { get; set; }
        
        [Display(Name = "Status")]
        public BlogStatus Status { get; set; }

        public virtual IdentityUser? User { get; set; }
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();


    }

}
