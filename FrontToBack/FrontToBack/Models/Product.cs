namespace FrontToBack.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }    
        //public string ImagePrimary { get; set; } 
        //public string ImageSecondary { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string SKU { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductImage>? ProductImages { get; set; }
    }
}
