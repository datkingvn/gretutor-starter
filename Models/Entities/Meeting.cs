using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreTutor.Models.Entities
{
    public class Meeting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; } // Tiêu đề cuộc họp

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } // Thời gian bắt đầu

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; } // Thời gian kết thúc

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Location { get; set; } // Địa điểm hoặc "Online"

        [Column(TypeName = "nvarchar(512)")]
        public string? MeetingLink { get; set; } // Link họp online (nếu có)

        [Column(TypeName = "nvarchar(1000)")]
        public string? Note { get; set; } // Ghi chú cuộc họp
    }
}