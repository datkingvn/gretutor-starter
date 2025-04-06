using System;
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
        public virtual Class? Class { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Title { get; set; } // Tiêu đề cuộc họp

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; } // Chỉ hiển thị ngày, giờ và phút

        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string Location { get; set; } // Địa điểm hoặc "Online"

        [Column(TypeName = "nvarchar(512)")]
        public string? MeetingLink { get; set; } // Link họp online (Google Meet, Zoom)

        [Column(TypeName = "nvarchar(512)")]
        public string? RecordingLink { get; set; } // 🔴 Link recording Google Meet (Tự động cập nhật)

        [Column(TypeName = "nvarchar(1000)")]
        public string? Note { get; set; } // 🔹 Ghi chú cuộc họp (nếu có)

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 🔹 Ngày tạo
    }
}
