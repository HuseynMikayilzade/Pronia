using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must enter a color name")]
        [MaxLength(50,ErrorMessage ="Maximum length can be 50")]
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }  
    }
}
