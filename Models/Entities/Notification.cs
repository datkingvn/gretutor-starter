using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreTutor.Models.Entities;

/// <summary>
/// Notification
/// </summary>
public class Notification
{
    /// <summary>
    /// Notification Id
    /// </summary>
    [Key]
    public int NotificationId { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Notification Message
    /// </summary>
    [Required]
    [Column(TypeName = "TEXT")]
    public string? Message { get; set; }

    /// <summary>
    /// Read Status
    /// </summary>
    public Boolean IsRead { get; set; } = false;

    /// <summary>
    /// Sent Date
    /// </summary>
    public DateTime? SentDate { get; set; } = DateTime.Today;

    /// <summary>
    /// User Navigation Property
    /// </summary>
    public ApplicationUser? User { get; set; }
}