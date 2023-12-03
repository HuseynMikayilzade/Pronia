using System.Net.Sockets;

namespace FrontToBack.ViewModels
{
    public class BasketItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public  string image { get; set; }
        public decimal Price { get; set; }
        public decimal SubTotal { get; set; }


    }
}
