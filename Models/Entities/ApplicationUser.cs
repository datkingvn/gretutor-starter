using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// Application User class
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User School Id
    /// </summary>
    [Required]
    public string? SchoolId { get; set; }

    /// <summary>
    /// Notifications List Navigational Property
    /// </summary>
    public IEnumerable<Notification>? Notifications { get; set; }
}