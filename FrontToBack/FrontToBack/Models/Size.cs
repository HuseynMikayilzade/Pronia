using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Size
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must enter a size")]
        [MaxLength(25,ErrorMessage ="Maximum length can be 25")]
        public string Name { get; set; }

        public List<ProductSize>? ProductSizes { get; set; }
    }
}
