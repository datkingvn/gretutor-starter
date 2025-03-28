using GreTutor.Models.Entities;
namespace GreTutor.Models.ViewModels
{
    public class ClassroomViewModel
    {
        public List<ClassMember> ClassMembers { get; set; } = new List<ClassMember>();
        public List<Document> Documents { get; set; } = new();

    }

}