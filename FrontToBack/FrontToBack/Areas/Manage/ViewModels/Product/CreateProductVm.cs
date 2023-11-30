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

        [Required(ErrorMessage = "You must enter a description")]
        [MaxLength(1000,ErrorMessage ="Maximum length can be 1000")]
        public string Description { get; set; }
        [Required(ErrorMessage ="You must enter a order")]
        public int Order { get; set; }
        public string SKU { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<int> ColorIds { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Color>? Colors { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public IFormFile MainPhoto { get; set; }
        public IFormFile HoverPhoto { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }

    }
}
