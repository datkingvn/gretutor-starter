using GreTutor.Models.ViewModels;
using System.Collections.Generic;

namespace GreTutor.Areas.Staff.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalAccounts { get; set; }
        public int TotalBlogPosts { get; set; }
        public int TotalClasses { get; set; }
        public int TotalStaffs { get; set; }
        public int TotalTutors { get; set; }
        public int TotalStudents { get; set; }

        public int PendingBlogPosts { get; set; }
        public int ApprovedBlogPosts { get; set; }
        public int RejectedBlogPosts { get; set; }

    }
}


