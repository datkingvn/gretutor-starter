using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// GreTutor Staff
/// </summary>
public class Staff
{
    /// <summary>
    /// Staff School Id
    /// </summary>
    [Key]
    public string? StaffId { get; set; }

    /// <summary>
    /// Identity User Id 
    /// </summary>
    /// <value></value>
    public string? UserId { get; set; }

    /// <summary>
    /// Identity User Navigation Property 
    /// </summary>
    /// <value></value>
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }
}