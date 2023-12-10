using FrontToBack.Models;

namespace FrontToBack.ViewModels
{
    public class OrderVm
    {
        public string Adress { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
       
    }
}
