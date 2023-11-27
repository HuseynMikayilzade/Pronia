using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class CreateSizeVM
    {
        [Required(ErrorMessage = "You must enter a color name")]
        [MaxLength(25, ErrorMessage = "Maximum length can be 25")]
        public string Name { get; set; }
    }
}
