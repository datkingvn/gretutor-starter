using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreTutor.Models.Entities
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [Required]
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual IdentityUser Sender { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(1000)")]
        public string Message { get; set; } // Nội dung tin nhắn

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Thời gian gửi
    }

}