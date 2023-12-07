using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Extention;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator,Designer")]
    [AutoValidateAntiforgeryToken]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = "Admin,Moderator,Designer")]
        public async Task<IActionResult> Index()
        {
            List<Product> products = await  _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            
            return View(products);
        }


        [Authorize(Roles = "Admin,Moderator,Designer")]
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
                if (!await _context.Colors.AnyAsync(c => c.Id == colorId))
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
                if (!await _context.Sizes.AnyAsync(s => s.Id == sizeId))
                {
                    createVm.Categories = await _context.Categories.ToListAsync();
                    createVm.Tags = await _context.Tags.ToListAsync();
                    createVm.Sizes = await _context.Sizes.ToListAsync();
                    createVm.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("SizeIds", "size duzgun deyil");
                    return View(createVm);
                }
            }

            foreach (var tagId in createVm.TagIds)
            { 
                if (!await _context.Tags.AnyAsync(t => t.Id == tagId))
                {
                    createVm.Categories = await _context.Categories.ToListAsync();
                    createVm.Tags = await _context.Tags.ToListAsync();
                    createVm.Sizes = await _context.Sizes.ToListAsync();
                    createVm.Colors = await _context.Colors.ToListAsync();
                    ModelState.AddModelError("TagIds", "tag duzgun deyil");
                    return View(createVm);
                }
            }

            if (!createVm.MainPhoto.CheckType("image/"))
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "The file type is incorrect");
                return View(createVm);
            }
            if (!createVm.HoverPhoto.CheckType("image/"))
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "The file type is incorrect");
                return View(createVm);
            }

            if (createVm.MainPhoto.CheckSize(3))
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("MainPhoto", "The file size is incorrect");
                return View(createVm);
            }
            if (createVm.HoverPhoto.CheckSize(3))
            {
                createVm.Categories = await _context.Categories.ToListAsync();
                createVm.Tags = await _context.Tags.ToListAsync();
                createVm.Sizes = await _context.Sizes.ToListAsync();
                createVm.Colors = await _context.Colors.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "The file size is incorrect");
                return View(createVm);
            }


            //==============================MainPhoto=================================//
            ProductImage mainphoto = new ProductImage
            {
                IsPrimary=true,
                Url = await createVm.MainPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images")
            };
            //==============================HoverPhoto=================================//
            ProductImage hoverphoto = new ProductImage
            {
                IsPrimary =false,
                Url = await createVm.HoverPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images")
            };

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
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImage> { mainphoto,hoverphoto},
                
            };

            //==============================AdditionalPhotos=================================//
            TempData["Message"] = "";
            foreach (var item in createVm.AdditionalPhotos ?? new List<IFormFile>()) //exception vermesin
            {
                if (!item.CheckType("image/"))
                {
                    TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\">{item.FileName} The file type is incorrect</div>";
                    continue;
                }
                if (item.CheckSize(2))
                {
                    TempData["Message"]+= $"<div class=\"alert alert-danger\" role=\"alert\">{item.FileName} The file size is incorrect</div>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Url =await item.CreateFileAsync(_env.WebRootPath,"assets","images","website-images")

                });
                
            }

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



        [Authorize(Roles = "Admin,Moderator,Designer")]
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductColors)
                .Include(p=>p.ProductSizes)
                .Include(p=>p.ProductImages)
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
                ProductImages=product.ProductImages,
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
            Product exist = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductColors)
                .Include(p=>p.ProductSizes)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(!ModelState.IsValid)
            {
                updateProductVm.Categories = await _context.Categories.ToListAsync();
                updateProductVm.Tags = await _context.Tags.ToListAsync();
                updateProductVm.Colors = await _context.Colors.ToListAsync();
                updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                updateProductVm.ProductImages = exist.ProductImages;
                return View(updateProductVm);
            }

            if (exist == null) return NotFound();

            if (updateProductVm.MainPhoto is not null)
            {
                if (!updateProductVm.MainPhoto.CheckType("image/"))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("MainPhoto", "The file type incorrect");
                    return View(updateProductVm);
                }
                if (updateProductVm.MainPhoto.CheckSize(3))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("MainPhoto", "The file size incorrect");
                    return View(updateProductVm);
                }               
            }
            if (updateProductVm.HoverPhoto is not null)
            {
                if (!updateProductVm.HoverPhoto.CheckType("image/"))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("HoverPhoto", "The file type incorrect");
                    return View(updateProductVm);
                }
                if (updateProductVm.HoverPhoto.CheckSize(3))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("HoverPhoto", "The file size incorrect");
                    return View(updateProductVm);
                }
            }

            bool result = await _context.Products.AnyAsync(p => p.Id == id);
            if (!result)
            {
                updateProductVm.Categories = await _context.Categories.ToListAsync();
                updateProductVm.Tags = await _context.Tags.ToListAsync();
                updateProductVm.Colors = await _context.Colors.ToListAsync();
                updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                updateProductVm.ProductImages = exist.ProductImages;
                ModelState.AddModelError("CategoryId", "Bele bir id-li product yoxdur");
                return View(updateProductVm);
            }
            foreach (var item in updateProductVm.TagIds)
            {
                if (! await _context.Tags.AnyAsync(t=>t.Id==item))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("TagIds", "bu idli tag yoxdur");
                    return View(updateProductVm);
                }
            }
            foreach (var item in updateProductVm.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(t => t.Id == item))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("SizeIds", "bu idli size yoxdur");
                    return View(updateProductVm);
                }
            }
            foreach (var item in updateProductVm.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(t => t.Id == item))
                {
                    updateProductVm.Categories = await _context.Categories.ToListAsync();
                    updateProductVm.Tags = await _context.Tags.ToListAsync();
                    updateProductVm.Colors = await _context.Colors.ToListAsync();
                    updateProductVm.Sizes = await _context.Sizes.ToListAsync();
                    updateProductVm.ProductImages = exist.ProductImages;
                    ModelState.AddModelError("ColorIds", "bu idli color yoxdur");
                    return View(updateProductVm);
                }
            }

            //==============================Size=================================//

            if (updateProductVm.SizeIds is not null)
            {
               exist.ProductSizes.RemoveAll(ps=> !updateProductVm.SizeIds.Exists(si=>si==ps.SizeId));

                foreach (var item in updateProductVm.SizeIds)
                {
                    if (!exist.ProductSizes.Any(ps=>ps.SizeId==item))
                    {
                        exist.ProductSizes.Add(new ProductSize { SizeId = item });
                    }
                }
            }

            //==============================Colors=================================//
            if (updateProductVm.ColorIds is not null)
            {
                exist.ProductColors.RemoveAll(pt => !updateProductVm.ColorIds.Exists(ti => ti == pt.ColorId));
                foreach (var colorId in updateProductVm.ColorIds)
                {
                    if (!exist.ProductColors.Any(pc=>pc.ColorId==colorId))
                    {
                        exist.ProductColors.Add(new ProductColor { ColorId = colorId });
                    }
                }
            }

            //==============================Tags=================================//

            if (updateProductVm.TagIds is not null)
            { 
                exist.ProductTags.RemoveAll(pt => !updateProductVm.TagIds.Exists(ti => ti == pt.TagId));
                foreach (var tagId in updateProductVm.TagIds)
                {
                    if (!exist.ProductTags.Any(pt=>pt.TagId==tagId))
                    {
                        exist.ProductTags.Add(new ProductTag {  TagId = tagId });
                    }
                }
            }


            //==============================Update Main and Hover Photos=================================//

            if (updateProductVm.MainPhoto is not null)
            {
                string fileName = await updateProductVm.MainPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images");
                ProductImage mainexistimg = exist.ProductImages.FirstOrDefault(pt => pt.IsPrimary == true);
                mainexistimg.Url.DeleteFile(_env.WebRootPath,"assets","images","website-images");
                exist.ProductImages.Remove(mainexistimg);

                exist.ProductImages.Add(new ProductImage { IsPrimary = true, Url = fileName });
             
            }

            if(updateProductVm.HoverPhoto is not null)
            {
                string fileName = await updateProductVm.HoverPhoto.CreateFileAsync(_env.WebRootPath,"assets","images","website-images");
                ProductImage hoverexistimg =exist.ProductImages.FirstOrDefault(pi=> pi.IsPrimary == false);
                hoverexistimg.Url.DeleteFile(_env.WebRootPath,"assets","images","website-images");
                exist.ProductImages.Remove(hoverexistimg);

                exist.ProductImages.Add(new ProductImage { IsPrimary=false, Url= fileName });
            }

            if (updateProductVm.ImageIds is null)
            {
                updateProductVm.ImageIds = new List<int>();
            }
            List<ProductImage> removableimg = exist.ProductImages.Where(pi=>!updateProductVm.ImageIds.Exists(ii=>ii==pi.Id)&&pi.IsPrimary==null).ToList();
            foreach (var item in removableimg)
            {
                item.Url.DeleteFile(_env.WebRootPath,"assets","images","website-images");
                exist.ProductImages.Remove(item);
            }
          
                TempData["Message"] = "";
                foreach (var item in updateProductVm.AdditionalPhotos ?? new List<IFormFile>()) //exception vermesin
                {
                    if (!item.CheckType("image/"))
                    {
                        TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\">{item.FileName} The file type is incorrect</div>";
                        continue;
                    }
                    if (item.CheckSize(2))
                    {
                        TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\">{item.FileName} The file size is incorrect</div>";
                        continue;
                    }
                    exist.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        Url = await item.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")

                    });
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            foreach (var item in product.ProductImages ?? new List<ProductImage>())
            {
                item.Url.DeleteFile(_env.WebRootPath,"assets","images","website-images");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }


        [Authorize(Roles = "Admin,Moderator,Designer")]
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

