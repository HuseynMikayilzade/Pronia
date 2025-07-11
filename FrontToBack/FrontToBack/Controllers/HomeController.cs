﻿using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Services;
using FrontToBack.Utilities.Exceptions;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FrontToBack.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
 

        public HomeController(AppDbContext context)
        {
            _context = context;
         
        }
        public async Task<IActionResult> Index()
        {
            
            //======================= Slides  ========================//
            List<Slide> slides = _context.Slides.OrderBy(s => s.Order).ToList();
            //if (slides == null) throw new  NotFoundException("slide tapilmadi");
          
            //======================= Services  ========================//
            List<CustomService> services = _context.CustomServices.ToList();

            //======================= Products ucun ========================//
            List<Product> products = _context.Products.Take(8).Include(p => p.ProductImages).ToList();

            //======================= Latest  ========================//
            List<Product> latestproducts = _context.Products.OrderByDescending(p => p.Id).Take(8).Include(p => p.ProductImages).ToList();

            //======================= NewProduct ========================//

            List<Product> newproducts = _context.Products.OrderByDescending(p => p.Id).Take(4).Include(p => p.ProductImages).ToList();

            HomeVM viewModel = new HomeVM
            {
                CustomServices = services,
                Products = products,
                LatestProducts = latestproducts,
                NewProducts = newproducts,
                Slides = slides,
            };

            return View(viewModel);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult ErrorPage(string error)
        {
            return View(model:error);
        }
    }
}
