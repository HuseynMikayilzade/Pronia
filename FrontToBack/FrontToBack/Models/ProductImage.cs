namespace FrontToBack.Models
{
    public class ProductImage
    {
       public int Id { get; set; }
        public string Url { get; set; }
        public bool? IsPrimary { get; set; } // ? 3 dene sekil vere bilmeyimiyiz ucun
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
