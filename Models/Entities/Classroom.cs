using System.ComponentModel.DataAnnotations;

namespace GreTutor.Models.Entities;

/// <summary>
/// Classroom information
/// </summary>
public class Classroom
{
    /// <summary>
    /// Classroom Id
    /// </summary>
    [Key]
    public int ClassroomId { get; set; }

    /// <summary>
    /// Class Tutor Id
    /// </summary>
    public string? TutorId { get; set; }

    /// <summary>
    /// Created Date
    /// </summary>
    public DateTime? CreatedDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Tutor Navigation Property
    /// </summary>
    public ApplicationUser? Tutor { get; set; }

    /// <summary>
    /// Students Navigation Property
    /// </summary>
    public IEnumerable<ApplicationUser>? Students { get; set; }

    /// <summary>
    /// Class Documents
    /// </summary>
    public IEnumerable<Document>? Documents { get; set; }
}