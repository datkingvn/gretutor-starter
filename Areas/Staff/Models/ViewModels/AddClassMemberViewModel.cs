using GreTutor.Models;
using System.Collections.Generic;

namespace GreTutor.Areas.Staff.Models.ViewModels
{
    public class AddClassMemberViewModel
    {
        public int ClassId { get; set; } // ID của lớp học
        public List<UserViewModel> Users { get; set; } // Danh sách Users chưa tham gia lớp
    }
}
