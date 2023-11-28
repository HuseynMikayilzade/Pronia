using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Migrations;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await  _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVm createProductVm = new CreateProductVm();
            createProductVm.Categories=await _context.Categories.ToListAsync();
            createProductVm.Tags = await _context.Tags.ToListAsync();
            createProductVm.Sizes = await _context.Sizes.ToListAsync();
            createProductVm.Colors = await _context.Colors.ToListAsync();
           
            return View(createProductVm);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm createVm)
        {
            if (!ModelState.IsValid)
            {
                createVm.Categories= await _context.Categories.ToListAsync();
                createVm.Tags= await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                return View(createVm);
            }
            if (createVm.Price < 0)
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("Price", "deyer duzgun deyil");
                return View(createVm);
            }

            bool categoryresult = await _context.Categories.AnyAsync(c => c.Id == createVm.CategoryId);

            if (!categoryresult)
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bu idli categori yoxdur");
                return View(createVm);
            }


            foreach (var colorId in createVm.ColorIds)
            {
                bool colorresult = await _context.Colors.AnyAsync(c => c.Id == colorId);
                if (!colorresult)
                {
                        createVm.Categories = await _context.Categories.ToListAsync();
                        createVm.Tags = await _context.Tags.ToListAsync();
                        createVm.Sizes = await _context.Sizes.ToListAsync();
                        createVm.Colors = await _context.Colors.ToListAsync();
                        ModelState.AddModelError("ColorIds", "color duzgun deyil");
                        return View(createVm);
                }
            }

            foreach (var sizeId in createVm.SizeIds)
            {
                bool sizeresult =await _context.Sizes.AnyAsync(s => s.Id == sizeId);

                if (!sizeresult)
                {
                    createVm.Categories = await _context.Categories.ToListAsync();
                    createVm.Tags = await _context.Tags.ToListAsync();
                    createVm.Sizes = await _context.Sizes.ToListAsync();
                    createVm.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "size duzgun deyil");
                    return View();
                }
            }

            foreach (var tagId in createVm.TagIds)
            {
                bool tagresult = await _context.Tags.AnyAsync(t => t.Id == tagId);
                if (tagresult)
                {
                    createVm.Categories = await _context.Categories.ToListAsync();
                    createVm.Tags = await _context.Tags.ToListAsync();
                    createVm.Sizes = await _context.Sizes.ToListAsync();
                    createVm.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("TagIds", "tag duzgun deyil");
                    return View();
                }
            }
           
            

            Product product = new Product
            {
                Name = createVm.Name,
                Price = createVm.Price,
                Order = createVm.Order,
                SKU = createVm.SKU,
                Description = createVm.Description,
                CategoryId = (int)createVm.CategoryId,
                ProductTags = new List<ProductTag>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            };

            //==============================Tags=================================//
          
            foreach (var tagId in createVm.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId=tagId
                };
               product.ProductTags.Add(productTag);
            }


            //==============================Colors=================================//
            foreach (var colorId in createVm.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId= colorId
                };
                product.ProductColors.Add(productColor);    
            }


            //==============================Sizes=================================//
            foreach (var sizeId in createVm.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId=sizeId
                };
                product.ProductSizes.Add(productSize);
            }


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductColors)
                .Include(p=>p.ProductSizes)
                .FirstOrDefaultAsync(p=>p.Id==id);
            if (product == null) return NotFound();
            UpdateProductVm updateProductVm = new UpdateProductVm
            {
                Name= product.Name,
                Price = product.Price,
                Order = product.Order,
                SKU = product.SKU,
                Description = product.Description,
                CategoryId =product.Id,
                Categories = await _context.Categories.ToListAsync(),  

                TagIds = product.ProductTags.Select(pt=>pt.TagId).ToList(),
                Tags = await _context.Tags.ToListAsync(),

                ColorIds=product.ProductColors.Select(pc=>pc.ColorId).ToList(),
                Colors = await _context.Colors.ToListAsync(),

                SizeIds = product.ProductSizes.Select(ps=>ps.SizeId).ToList(),
                Sizes = await _context.Sizes.ToListAsync(),
            };
            return View(updateProductVm); 
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVm updateProductVm)
        {
            if(!ModelState.IsValid)
            {
                updateProductVm.Categories = await _context.Categories.ToListAsync();
                return View(updateProductVm);
            }

            Product exist = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductColors)
                .Include(p=>p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (exist == null) return NotFound();

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result)
            {
                updateProductVm.Categories = await _context.Categories.ToListAsync();
                ModelState.AddModelError("CategoryId", "Bele bir id-li product yoxdur");
                return View(updateProductVm);
            }

            //==============================Size=================================//

            foreach (var psize in exist.ProductSizes)
            {
                if (!updateProductVm.SizeIds.Exists(ti=>ti==psize.Id))
                {
                    _context.ProductSizes.Remove(psize);
                }
            }
            foreach (var sizeId in updateProductVm.SizeIds)
            {
                if (! exist.ProductSizes.Any(ps=>ps.SizeId==sizeId))
                {
                    exist.ProductSizes.Add(new ProductSize { SizeId = sizeId, });
                }
            }
            //==============================Colors=================================//

            foreach (var pcolor in exist.ProductColors)
            {
                if (!updateProductVm.ColorIds.Exists(ci=>ci==pcolor.Id))
                {
                    _context.ProductColors.Remove(pcolor);
                }
            }
            foreach (var colorId in updateProductVm.ColorIds)
            {
                if (!exist.ProductColors.Any(pc=>pc.ColorId==colorId))
                {
                    exist.ProductColors.Add(new ProductColor { ColorId = colorId });
                }
            }

            //==============================Tags=================================//

            foreach (var ptag in exist.ProductTags)
            {
                if (!updateProductVm.TagIds.Exists(ti=>ti==ptag.Id))
                {
                    _context.ProductTags.Remove(ptag);
                }
            }
            foreach (var tagId in updateProductVm.TagIds)
            {
                if (!exist.ProductTags.Any(pt=>pt.TagId==tagId))
                {
                    exist.ProductTags.Add(new ProductTag {  TagId = tagId });
                }
            }


            exist.SKU=updateProductVm.SKU;
            exist.Description=updateProductVm.Description;
            exist.Price=updateProductVm.Price;
            exist.Order=updateProductVm.Order;
            exist.Name=updateProductVm.Name;
            exist.CategoryId =updateProductVm.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        public async Task<IActionResult> Detail(int id)
        {
           Product product = await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductColors).ThenInclude(pc=>pc.Color)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .Include(p=>p.ProductSizes).ThenInclude(pt=>pt.Size)
                .Include(p=>p.ProductImages).FirstOrDefaultAsync(p=>p.Id==id);
               

            if (product == null) return NotFound();
           
            return View(product);
        }
    }
}

