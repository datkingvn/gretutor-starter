using System.ComponentModel.DataAnnotations;

namespace GreTutor.Models.Entities;

/// <summary>
/// Meeting Document
/// </summary>
public class Document
{
    /// <summary>
    /// Document Id
    /// </summary>
    [Key]
    public int DocumentId { get; set; }

    /// <summary>
    /// Uploader Id (Student or Tutor)
    /// </summary>
    public string? UploaderId { get; set; }

    /// <summary>
    /// Classroom Id
    /// </summary>
    public int? ClassroomId { get; set; }

    /// <summary>
    /// Document Meeting Id
    /// </summary>
    public int? MeetingId { get; set; }

    /// <summary>
    /// Document File Name
    /// </summary>
    [Required]
    public string? FileName { get; set; }

    /// <summary>
    /// Upload Date
    /// </summary>
    public DateTime? UploadDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Uploader Identity User Navigation Property
    /// </summary>
    public ApplicationUser? Uploader { get; set; }

    /// <summary>
    /// Classroom Navigation Property
    /// </summary>
    public Classroom? Classroom  { get; set; }

    /// <summary>
    /// Meeting Navigation Property
    /// </summary>
    public Meeting? Meeting  { get; set; }
}