using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Controllers
{
    public class ProductController:Controller
    {
        private readonly AppDbContext _context;
        public ProductController (AppDbContext context)
        {
             _context = context;
        }
        public IActionResult Detail (int id)
        {

            if (id <= 0) return BadRequest();
            
            //======================= Product ========================//
            Product product= _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .FirstOrDefault(p => p.Id == id);

            //======================= Related Products  ========================//

            List<Product> relatedproducts = _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Where(p=>p.CategoryId == product.CategoryId && p.Id!=id).ToList();

            if (product == null) return NotFound();

            //======================= Services  ========================//
            List<CustomService> customService=_context.CustomServices.ToList();
            
            HomeVM homeVM = new HomeVM{
                Product=product,
                CustomServices=customService,
                RelatedProducts=relatedproducts
            };
            
            return View(homeVM); 
        }
    }
}
