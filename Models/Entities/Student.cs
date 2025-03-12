using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// GreTutor Student
/// </summary>
public class Student
{
    /// <summary>
    /// Student School Id
    /// </summary>
    [Required]
    public string? SchoolId { get; set; }

    /// <summary>
    /// Identity User Id 
    /// </summary>
    /// <value></value>
    public string? UserId { get; set; }
 
    /// <summary>
    /// Enrollment Year 
    /// </summary>
    [Required]
    public DateTime? EnrollmentYear { get; set; }

    /// <summary>
    /// Identity User Navigation Property 
    /// </summary>
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
}