using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Exceptions;
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
        public async Task<IActionResult> Detail (int id)
        {

            if (id <= 0) throw new BadRequestException(" Bad Request :(");
               
   
            //======================= Product ========================//
            Product product= _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductSizes).ThenInclude(ps=>ps.Size)
                .FirstOrDefault(p => p.Id == id);

            //======================= Related Products  ========================//

            if (product == null) throw new NotFoundException("Product Not Found :(");

            List<Product> relatedproducts = _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .Where(p=>p.CategoryId == product.CategoryId && p.Id!=id).ToList();

            if (relatedproducts == null) throw new NotFoundException("Product Not Found :(");


            //======================= Services  ========================//
            List<CustomService> customService=_context.CustomServices.ToList();

            DetailVm detailVm = new DetailVm
            {
                Product= product,
                CustomServices= customService,
                RelatedProducts=await _context.Products.Where(p=>p.CategoryId==product.CategoryId && p.Id!=id)
                .Take(8).Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).ToListAsync()
            };

            
            return View(detailVm); 
        }
    }
}
