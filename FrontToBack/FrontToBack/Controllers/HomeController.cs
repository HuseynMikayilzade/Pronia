using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<CustomService> services = new List<CustomService> 
            { 
                new CustomService
                {
                    Id = 1,
                    Title = "frees hipping",
                    Description = "description text",
                    Icon = "car.png"
                   
                },
                 new CustomService
                {
                    Id = 2,
                    Title = "safe payment",
                    Description = "description text",
                    Icon = "card.png"

                },
                  new CustomService
                {
                    Id = 3,
                    Title = "best services",
                    Description = "description text",
                    Icon = "service.png"

                }
            };
            List<Product> products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "american marigold",
                    Price = 15,
                    ImagePrimary ="1-1-270x300.jpg",
                    ImageSecondary ="1-2-270x300.jpg"
                }, 
                new Product
                {
                    Id = 2,
                    Name = "black eyed susan",
                    Price = 16,
                    ImagePrimary ="1-3-270x300.jpg",
                    ImageSecondary ="1-4-270x300.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "bleedin heart",
                    Price = 17,
                    ImagePrimary ="1-5-270x300.jpg",
                    ImageSecondary ="1-6-270x300.jpg"
                },
                new Product
                {
                    Id = 4,
                    Name = "bloody",
                    Price = 18,
                    ImagePrimary ="1-7-270x300.jpg",
                    ImageSecondary ="1-8-270x300.jpg"
                }, 
                
                new Product
                {
                    Id = 5,
                    Name = "buttefly",
                    Price = 19,
                    ImagePrimary ="1-3-270x300.jpg",
                    ImageSecondary ="1-4-270x300.jpg"
                },
                new Product
                {
                    Id = 6,
                    Name = "common",
                    Price = 20,
                   ImagePrimary ="1-1-270x300.jpg",
                    ImageSecondary ="1-2-270x300.jpg"
                },
                new Product
                {
                    Id = 7,
                    Name = "viburnum",
                    Price = 21,
                    ImagePrimary ="1-3-270x300.jpg",
                    ImageSecondary ="1-5-270x300.jpg"
                },
                new Product
                {
                    Id = 8,
                    Name = "reed",
                    Price = 22,
                    ImagePrimary ="1-1-270x300.jpg",
                    ImageSecondary ="1-2-270x300.jpg"
                }
            };

            
            ViewModel viewModel = new ViewModel
            {
                CustomService = services,
                Product = products   
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
