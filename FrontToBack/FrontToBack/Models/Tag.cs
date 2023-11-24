using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Tag
    {
        public int Id { get; set; }
       
        [Required(ErrorMessage = "You must enter a color name")]
        [MaxLength(25,ErrorMessage = "Maximum length can be 25")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
        
    }
}
