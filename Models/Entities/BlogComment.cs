using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// Blog Comment
/// </summary>
public class BlogComment : Comment
{
    /// <summary>
    /// Blog ID
    /// </summary>
    public int BlogId { get; set; }

    /// <summary>
    /// Blog Post Navigation Property
    /// </summary>
    [ForeignKey("BlogId")]
    public virtual BlogPost? Blog { get; set; }
}