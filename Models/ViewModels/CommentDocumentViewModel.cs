using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GreTutor.Models.ViewModels
{
    public class CommentDocumentViewModel
    {
        public int DocumentId { get; set; }
        public string Content { get; set; }
    }

}