using FrontToBack.Utilities.Enum;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class RegisterVm
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length can be 3")]
        [MaxLength(25, ErrorMessage = "Maximum length can be 25")]
        public string Name { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length can be 3")]
        [MaxLength(25, ErrorMessage = "Maximum length can be 25")]
        public string Surname { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length can be 3")]
        [MaxLength(25, ErrorMessage = "Maximum length can be 25")]
        public string Username { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length can be 3")]
        [MaxLength(30, ErrorMessage = "Maximum length can be 30")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string Confirmpassword { get; set; }
        [Required]
        public Gender Gender { get; set; }


    }
}
