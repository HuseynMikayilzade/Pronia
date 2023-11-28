using FrontToBack.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Areas.Manage.ViewModels
{
    public class CreateProductVm
    {
        [Required(ErrorMessage = "You must enter a Product name")]
        [MaxLength(50, ErrorMessage = "Maximum length can be 50")]

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string SKU { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<int> ColorIds { get; set; }

        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }



        [Required]
        public int? CategoryId { get; set; }
      


    }
}
