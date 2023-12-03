using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;

namespace FrontToBack.Controllers
{
    public class BasketController:Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVm> itemvm = new List<BasketItemVm>();

            if (Request.Cookies["Basket"] != null)
            {
                List<BasketCookieItemVm> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
                if (cookies !=null)
                {
                    foreach (var item in cookies)
                    {
                        Product product = await _context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (cookies !=null)
                        {
                            BasketItemVm basketItemVm = new BasketItemVm
                            {
                                
                                Id = product.Id,
                                Name = product.Name, 
                                Price = product.Price,
                                image= product.ProductImages.FirstOrDefault().Url,
                                Count =item.Count,
                                SubTotal = item.Count*product.Price,
                            };
                            itemvm.Add(basketItemVm);
                        }    
                    }
                }               
            }
            return View(itemvm);
        }



        //=======================================AaddBasket=====================================//

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            List<BasketCookieItemVm> basket;
            if (Request.Cookies["Basket"] is null)
            {
                basket = new List<BasketCookieItemVm>();
                BasketCookieItemVm basketCookieItem = new BasketCookieItemVm
                {
                    Id = id,
                    Count = 1
                };
                basket.Add(basketCookieItem);
            }
            else
            {               
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
                BasketCookieItemVm existed = basket.FirstOrDefault(b=>b.Id==id);

                if (existed == null)
                {
                      BasketCookieItemVm basketCookieItemVm = new BasketCookieItemVm
                      {
                          Id = id,
                          Count=1
                      };
                      basket.Add(basketCookieItemVm);
                }
                else
                {
                    existed.Count++;
                }
               
            }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index),"Home");        
        }


        //=======================================Remove=====================================//

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();   
            List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
            if (basket == null) return BadRequest();
            
            BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);

                
            basket.Remove(existed);

            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);

            return RedirectToAction(nameof(Index));
        }


        //=======================================PlusBasket=====================================//

        public async Task<IActionResult> PlusBasket(int id)
        {
            if (id <= 0) return BadRequest();

            List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
            if (basket == null) return NotFound();

            BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);

            if (existed != null)
            {
                if (existed.Count >0)
                {

                    existed.Count++;
                }
                else
                {
                    basket.Add(existed);
                }
            }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }


        //=======================================MinusBasket=====================================//
        public async Task<IActionResult> MinusBasket(int id)
        {
            if (id <= 0) return BadRequest();
          
            List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
            if (basket == null) return NotFound();

            BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);

            if (existed != null)
            {
                if (existed.Count > 1)
                {
                   
                    existed.Count--;
                }
                else
                {
                    basket.Remove(existed);
                }
            }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
    }
}
