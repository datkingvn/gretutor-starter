using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GreTutor.Models.Entities;
namespace GreTutor.Models.ViewModels
{
    public class DocumentViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public IFormFile File { get; set; }

        public int ClassId { get; set; }

        // Thêm thuộc tính DocumentId vào ViewModel
        public int DocumentId { get; set; }
        public ICollection<CommentDocument> CommentDocuments { get; set; } = new List<CommentDocument>();
    }


}