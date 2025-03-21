using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using GreTutor.Models.Enums;

namespace GreTutor.Models.Entities;

/// <summary>
/// Blog Post Entity
/// </summary>
public class BlogPost
{
    /// <summary>
    /// Blog ID
    /// </summary>
    /// <value></value>
    [Key]
    public int BlogId { get; set; }

    /// <summary>
    /// Blog Title
    /// </summary>
    /// <value></value>
    [Required]
    [Column(TypeName = "nvarchar(255)")]
    public string? Title { get; set; }

    /// <summary>
    /// Blog Content
    /// </summary>
    /// <value></value>
    [Required]
    [Column(TypeName = "TEXT")]
    public string? Content { get; set; }

    /// <summary>
    /// Blog Created Time
    /// </summary>
    /// <value></value>
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime Created { get; set; } = DateTime.Now;

    /// <summary>
    /// Blog Identity Author Id
    /// </summary>
    public string? AuthorId { get; set; }

    /// <summary>
    /// Blog Status
    /// </summary>
    [Display(Name = "Status")]
    public BlogStatus Status { get; set; }

    /// <summary>
    ///  Identity User Navigation Property
    /// </summary>
    [ForeignKey("AuthorId")]
    public virtual ApplicationUser? User { get; set; }

    /// <summary>
    /// Blog Comments Navigation Property 
    /// </summary>
    public virtual IEnumerable<BlogComment>? Comments { get; set; }
}
