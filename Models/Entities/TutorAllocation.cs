using System.ComponentModel.DataAnnotations;

namespace GreTutor.Models.Entities;

/// <summary>
/// Tutor Allocation Informations
/// </summary>
public class TutorAllocation
{
    /// <summary>
    /// Tutor Allocation Id
    /// </summary>
    [Key]
    public int AllocationId { get; set; }

    /// <summary>
    /// Student Id
    /// </summary>
    public string? StudentId { get; set; }

    /// <summary>
    /// Tutor Id
    /// </summary>
    public string? TutorId { get; set; }

    /// <summary>
    /// Staff Id
    /// </summary>
    public string? AssignedBy { get; set; }

    /// <summary>
    /// Assigned Date
    /// </summary>
    public DateTime? AssignedDate { get; set; } = DateTime.Today;

    /// <summary>
    /// Student Navigation Property
    /// </summary>
    public ApplicationUser? Student { get; set; }

    /// <summary>
    /// Tutor Navigation Property
    /// </summary>
    public ApplicationUser? Tutor { get; set; }

    /// <summary>
    /// Staff Navigation Property
    /// </summary>
    public ApplicationUser? Staff { get; set; }
}