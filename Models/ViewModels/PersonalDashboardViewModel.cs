using GreTutor.Models.ViewModels;
using System.Collections.Generic;

namespace GreTutor.Models.ViewModels
{
    public class PersonalDashboardViewModel
    {
        public int PendingBlogPosts { get; set; }
        public int ApprovedBlogPosts { get; set; }
        public int RejectedBlogPosts { get; set; }
        public int TotalMeetings { get; set; }
        public int TotalDocuments { get; set; }
    }
}


