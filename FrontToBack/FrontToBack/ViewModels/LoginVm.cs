using System.ComponentModel.DataAnnotations;

namespace FrontToBack.ViewModels
{
    public class LoginVm
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum length can be 3")]
        [MaxLength(80, ErrorMessage = "Maximum length can be 80")]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool İsRemember { get; set; }

    }
}
