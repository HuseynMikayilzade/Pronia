using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontToBack.Models
{
    public class Slide
    {
       
        public int Id { get; set; }

        public string subTitle { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string? Image { get; set; }
        public int Order { get; set; }

        [NotMapped]
        public IFormFile? Photo { get; set; }
    }
}
