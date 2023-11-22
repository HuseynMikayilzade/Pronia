using FrontToBack.Models;

namespace FrontToBack.Areas.Manage.ViewModel
{
    public class DashboardVm
    {
        public List<Category> Categories { get; set; }
        public List<Color> Colors { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Size> Sizes { get; set; }


    }
}
