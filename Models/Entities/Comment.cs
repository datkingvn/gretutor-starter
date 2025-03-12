using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// User Comment
/// </summary>
public abstract class Comment
{

    /// <summary>
    /// Comment Id
    /// </summary>
    [Key]
    public int CommentId { get; set; }

    /// <summary>
    ///  Comment Content
    /// </summary>
    [Required]
    [Column(TypeName = "TEXT")]
    public string? Content { get; set; }

    /// <summary>
    /// Comment Created Time 
    /// </summary>
    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime Created { get; set; } = DateTime.Now;

    /// <summary>
    /// Comment Author Id
    /// </summary>
    public string? AuthorId { get; set; }

    /// <summary>
    /// Author Identity User Navigation Property 
    /// </summary>
    [ForeignKey("AuthorId")]
    public virtual ApplicationUser? User { get; set; }
}
