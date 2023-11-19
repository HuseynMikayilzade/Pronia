using FrontToBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.ViewModels
{
    public class HomeVM
    {
        public List<CustomService> CustomServices { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> LatestProducts { get; set; }
        public List<Product> NewProducts { get; set; }

        public List<Slide> Slides { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
        public List<Product> RelatedProducts { get; set; }



    }
}
