using FrontToBack.DAL;
using FrontToBack.Interfaces;
using FrontToBack.Models;
using FrontToBack.Services;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;
using FrontToBack.Utilities.Extentions;
using Microsoft.Extensions.Hosting;
using FrontToBack.Utilities.Exceptions;

namespace FrontToBack.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public BasketController(AppDbContext context, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVm> itemvm = new List<BasketItemVm>();

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) throw new NotFoundException("User Not Found ");
                foreach (var item in appUser.BasketItems)
                {
                    itemvm.Add(new BasketItemVm
                    {
                        Id = item.ProductId,
                        Price = item.Product.Price,
                        Count = item.Count,
                        Name = item.Product.Name,
                        SubTotal = item.Count * item.Product.Price,
                        image = item.Product.ProductImages.FirstOrDefault()?.Url
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
                            if (product == null) throw new NotFoundException("Product Not Found ");

                            if (cookies != null)
                            {

                                itemvm.Add(new BasketItemVm
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Price = product.Price,
                                    image = product.ProductImages.FirstOrDefault().Url,
                                    Count = item.Count,
                                    SubTotal = item.Count * product.Price,

                                });
                            }
                        }
                    }
                }
            }
            return View(itemvm);
        }

        //=======================================AaddBasket=====================================//

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found ");

            if (User.Identity.IsAuthenticated)
                {
                    AppUser appuser = await _userManager.Users
                        .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (appuser == null) throw new NotFoundException("User Not Found ");
                    BasketItem basketItem = appuser.BasketItems.FirstOrDefault(p => p.ProductId == product.Id);
                    if (basketItem == null)
                    {
                        basketItem = new BasketItem
                        {
                            AppUserId = appuser.Id,
                            ProductId = product.Id,
                            Count = 1,
                            Price = product.Price,
                        };
                        appuser.BasketItems.Add(basketItem);

                    }
                    else
                    {
                        basketItem.Count++;
                    }
                    await _context.SaveChangesAsync();
            }
                else
                {
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
                        BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);

                        if (existed == null)
                        {
                            BasketCookieItemVm basketCookieItemVm = new BasketCookieItemVm
                            {
                                Id = id,
                                Count = 1
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
                }
            return RedirectToAction(nameof(Index), "Home");
        }


        //=======================================Remove=====================================//

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found :(");

            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (appuser == null) throw new NotFoundException("User Not Found :(");
                BasketItem basketItem = appuser.BasketItems.FirstOrDefault(b => b.ProductId == product.Id);
                if (basketItem == null) throw new NotFoundException("An Unexpected Error Occurred :(");
                appuser.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
                if (basket == null) throw new NotFoundException("An Unexpected Error Occurred :(");

                BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);
                if (existed == null) throw new NotFoundException("An Unexpected Error Occurred :(");
                basket.Remove(existed);
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);

            }

            return RedirectToAction(nameof(Index));
        }


        //=======================================PlusBasket=====================================//

        public async Task<IActionResult> PlusBasket(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = await _context.Products.FirstOrDefaultAsync(bi => bi.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found :(");


            if (User.Identity.IsAuthenticated)
            {
                AppUser appuser = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appuser == null) throw new NotFoundException("User Not Found :(");
                BasketItem basketItem = appuser.BasketItems.FirstOrDefault(bi => bi.ProductId == product.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        Price = product.Price,
                        ProductId = product.Id,
                        Count = 1,
                        AppUserId = appuser.Id,
                    };
                }
                else
                {
                    basketItem.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVm> basket = JsonConvert.DeserializeObject<List<BasketCookieItemVm>>(Request.Cookies["Basket"]);
                if (basket == null) throw new NotFoundException("An Unexpected Error Occurred :(");

                BasketCookieItemVm existed = basket.FirstOrDefault(b => b.Id == id);

                if (existed != null)
                {
                    if (existed.Count > 0)
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
            }
            return RedirectToAction(nameof(Index), "Basket");
        }


        //=======================================MinusBasket=====================================//
        public async Task<IActionResult> MinusBasket(int id)
        {
            if (id <= 0) throw new BadRequestException(" Bad Request :(");
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) throw new NotFoundException("Product Not Found :(");

            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (appUser == null) throw new NotFoundException("User Not Found :(");


                BasketItem basketitem = appUser.BasketItems.FirstOrDefault(bi => bi.ProductId == product.Id);
                if (basketitem == null)
                {
                    basketitem = new BasketItem
                    {
                        ProductId = product.Id,
                        AppUserId = appUser.Id,
                        Price = product.Price,
                        Count = basketitem.Count
                    };
                }
                else
                {
                    basketitem.Count--;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
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
            }
            return RedirectToAction(nameof(Index), "Basket");

        }



        //=======================================CheckOut=====================================//

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CheckOut()
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (appUser == null) throw new NotFoundException("User Not Found :(");
            OrderVm ordervm = new OrderVm
            {
                BasketItems = appUser.BasketItems,

            };
            return View(ordervm);
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut(OrderVm orderVm)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi => bi.OrderId == null)).ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (appUser == null) throw new NotFoundException("User Not Found :(");

            if (!ModelState.IsValid)
            {
                orderVm.BasketItems = appUser.BasketItems;

                return View(orderVm);
            }

            decimal total = 0;
            foreach (var item in appUser.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Price * item.Count;
            }

            Order order = new Order
            {
                Adress = orderVm.Adress,
                AppUserId = appUser.Id,
                Status = null,
                Received = DateTime.Now,
                BasketItems = appUser.BasketItems,
                TotalPrice = total
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your order has been completed successfully. Thanks!";

            string emailbody = EmailBodyCreator.EmailBody(appUser);
            await _emailService.SendEmailAsync(appUser.Email, "Test Subject", emailbody, true);

            return RedirectToAction(nameof(CheckOut), "Basket");
        }
    }
}




