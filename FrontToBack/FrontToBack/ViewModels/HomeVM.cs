using FrontToBack.Models;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.ViewModels
{
    public class HomeVM
    {
        public List<Product> Products { get; set; }
        public List<Product> LatestProducts { get; set; }
        public List<Product> NewProducts { get; set; }

        public List<Slide> Slides { get; set; }
 
        public Category Category { get; set; }
        public List<CustomService> CustomServices { get; set; }

    }
}
