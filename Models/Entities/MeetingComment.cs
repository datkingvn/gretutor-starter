using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GreTutor.Models.Entities;

/// <summary>
/// Meeting Comment
/// </summary>
public class MeetingComment : Comment
{
    /// <summary>
    /// Meeting ID
    /// </summary>
    public int MeetingId { get; set; }

    /// <summary>
    /// Meeting Navigation Property
    /// </summary>
    [ForeignKey("MeetingId")]
    public virtual Meeting? Meeting  { get; set; }
}