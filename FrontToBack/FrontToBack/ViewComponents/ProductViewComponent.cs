﻿using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public ProductViewComponent(AppDbContext context)
        {

            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int key=1)
        {
            List<Product> products;
            switch (key)
            {
                case 1:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
                case 2:
                    products = await _context.Products.Take(8).OrderByDescending(p=>p.Price).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

                    break;
                case 3:
                    products = await _context.Products.Take(8).OrderByDescending(p => p.Id).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

                    break;
                default:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

                    break;
            }
            return View(products);
        }
    }
}
