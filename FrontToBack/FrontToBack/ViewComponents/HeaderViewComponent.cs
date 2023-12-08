using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FrontToBack.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public HeaderViewComponent(AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor http)
        {
            _context = context;
            _userManager = userManager;
            _http =http;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> setting = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            List<BasketItemVm> itemvm = new List<BasketItemVm>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems).ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (var item in appUser.BasketItems)
                {
                    itemvm.Add(new BasketItemVm
                    {
                        Id = item.ProductId,
                        Price = item.Product.Price,
                        Name = item.Product.Name,
                        Count = item.Count,
                        image = item.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true)?.Url,
                        SubTotal = item.Count * item.Product.Price
                    });
                }
            }
            else
            {
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

