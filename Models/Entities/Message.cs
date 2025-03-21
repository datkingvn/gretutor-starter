using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreTutor.Models.Entities;

/// <summary>
/// Private Message Entity
/// </summary>
public class Message
{
    /// <summary>
    /// Message Id
    /// </summary>
    [Key]
    public int MessageId { get; set; }

    /// <summary>
    /// Sender Identity ID
    /// </summary>
    public string? SenderId { get; set; }

    /// <summary>
    /// Receiver Identity Id
    /// </summary>
    public string? ReceiverId { get; set; }

    /// <summary>
    /// Content
    /// </summary>
    [Required]
    [Column(TypeName = "TEXT")]
    public string? Content { get; set; }

    /// <summary>
    /// Send Time
    /// </summary>
    public DateTime? TimeStamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Sender Navigation Property
    /// </summary>
    /// <value></value>
    public virtual ApplicationUser? Sender { get; set; }

    /// <summary>
    /// Receiver Navigation Property 
    /// </summary>
    public virtual ApplicationUser? Receiver { get; set; }
}