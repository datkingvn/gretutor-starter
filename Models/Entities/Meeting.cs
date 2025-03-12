using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GreTutor.Models.Enums;

namespace GreTutor.Models.Entities;

/// <summary>
/// Meeting Information
/// </summary>
public class Meeting
{
    /// <summary>
    /// Meeting Id
    /// </summary>
    [Key]
    public int MeetingId { get; set; }

    /// <summary>
    /// Tutor ID
    /// </summary>
    [Required]
    public string? TutorId { get; set; }

    /// <summary>
    /// Meeting Time
    /// </summary>
    [Required]
    public DateTime? DateTime { get; set; }

    /// <summary>
    /// Meeting Type
    /// </summary>
    [Required]
    public MeetingType MeetingType { get; set; }

    /// <summary>
    /// Meeting Note
    /// </summary>
    /// <value></value>
    [Column(TypeName = "TEXT")] 
    public string? Note { get; set; }

    /// <summary>
    /// Meeting Student List Navigation Property
    /// </summary>
    public virtual IEnumerable<ApplicationUser>? Students { get; set; }

    /// <summary>
    /// Meeting Tutor Navigation Property
    /// </summary>
    public virtual ApplicationUser? Tutor { get; set; }
}