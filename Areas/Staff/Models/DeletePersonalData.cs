using System.ComponentModel.DataAnnotations;

namespace GreTutor.Areas.Staff.Models
{
    public class DeletePersonalDataModel
    {
        public string UserId { get; set; }
        public bool RequirePassword { get; set; }

        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
