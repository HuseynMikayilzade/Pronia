using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class DetailVm
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<CustomService> CustomServices { get; set; }
        public List<Product> Products { get; set; }

    }
}
