using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class UpdateSlideVm
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
