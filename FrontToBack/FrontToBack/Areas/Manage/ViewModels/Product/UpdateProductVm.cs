using FrontToBack.Models;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class UpdateProductVm
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<int> ColorIds { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }


    }
}
