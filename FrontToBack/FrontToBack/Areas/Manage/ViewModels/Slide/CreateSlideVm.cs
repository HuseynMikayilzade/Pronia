using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Areas.Manage.ViewModels.Slide
{
    public class CreateSlideVm
    {
        [Required]

        public string subTitle { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public int Order { get; set; }

        [Required]
        public IFormFile? Photo { get; set; }
    }
}
