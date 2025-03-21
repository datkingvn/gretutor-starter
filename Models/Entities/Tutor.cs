using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// GreTutor Tutor
/// </summary>
public class Tutor
{
    /// <summary>
    /// Tutor School Id
    /// </summary>
    [Key]
    public string? TutorId { get; set; }

    /// <summary>
    /// Identity User Id 
    /// </summary>
    /// <value></value>
    public string? UserId { get; set; }

    /// <summary>
    /// Department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Identity User Navigation Property 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
}