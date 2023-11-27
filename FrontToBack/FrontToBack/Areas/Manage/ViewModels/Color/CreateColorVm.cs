using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class CreateColorVm
    {
        [Required(ErrorMessage = "You must enter a color name")]
        [MaxLength(30, ErrorMessage = "Maximum length can be 50")]
        public string Name { get; set; }
    }
}
