using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> setting = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            List<BasketItemVm> itemvm = new List<BasketItemVm>();

            if (Request.Cookies["Basket"] != null)
            {
                List<BasketCookieItemVm> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
                if (cookies != null)
                {

                    foreach (var item in cookies)
                    {
                        Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (cookies != null)
                        {
                            BasketItemVm basketItemVm = new BasketItemVm
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Price = product.Price,
                                image = product.ProductImages.FirstOrDefault().Url,
                                Count = item.Count,
                                SubTotal = item.Count * product.Price,
                            };
                            itemvm.Add(basketItemVm);
                        }

                    }
                }

            }
            HeaderBasketVm headerBasketVm = new HeaderBasketVm
            {
                BasketItemVm = itemvm,
                Settings = setting
            };
            return View(headerBasketVm);
        }
    }
}

