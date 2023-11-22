using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must enter a category name")]
        [MaxLength(25,ErrorMessage = "Maximum length can be 25")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
