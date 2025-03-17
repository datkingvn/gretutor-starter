using GreTutor.Models.Entities;
namespace GreTutor.Models.ViewModels
{
    public class ClassroomViewModel
    {
        // Các lớp mà user tham gia (qua bảng ClassMember, bao gồm thông tin lớp)
        public List<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        public List<Document> Documents { get; set; } = new();

        // Có thể mở rộng thêm các thông tin khác nếu cần
    }

}