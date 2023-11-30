using FrontToBack.Areas.Manage.ViewModels;
using FrontToBack.Areas.Manage.ViewModels.Slide;
using FrontToBack.DAL;
using FrontToBack.Models;
using FrontToBack.Utilities.Extention;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }

        //========================================== Create =======================================//

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVm slideVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!slideVm.Photo.CheckType("image/"))  
            {
                ModelState.AddModelError("Photo", "faylin tipi uygun deyil");
                return View();
            }
            if (slideVm.Photo.CheckSize(3))
            {
                ModelState.AddModelError("Photo", "Seklin olcusu 3mb-den artiq olmamalidir.");
                return View();
            }




            string filename = await slideVm.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider","slide-img");

            Slide slide = new Slide
            {
                Desc = slideVm.Desc,
                Title = slideVm.Title,
                subTitle = slideVm.subTitle,
                Order = slideVm.Order,
            };
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //========================================== Detail =======================================//

        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            return View(existed);


        }

        //========================================== Delete =======================================//

        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return NotFound();
            Slide exist = await _context.Slides.FirstOrDefaultAsync(slide => slide.Id == id);
            if (exist == null) return NotFound();

            exist.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider", "slide-img");
           
            _context.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //========================================== Update =======================================//

        public async Task<IActionResult> Update(int id)
        {
            if (id == null) return NotFound();


            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            UpdateSlideVm updateSlideVm = new UpdateSlideVm
            {
                Desc = existed.Desc,
                Order = existed.Order,
                subTitle =existed.subTitle,
                Title = existed.Title,
                Photo = existed.Photo
                
            };

            return View(updateSlideVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSlideVm updateSlideVm)
        {
            Slide exist =await _context.Slides.FirstOrDefaultAsync(s=>s.Id == id);
            if (exist == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (updateSlideVm.Photo != null)
            {
                    if (!updateSlideVm.Photo.CheckType("image/"))
                    {
                        ModelState.AddModelError("Photo", "faylin tipi uygun deyil");
                        return View();
                    }
                    if (updateSlideVm.Photo.CheckSize(3))
                    {
                        ModelState.AddModelError("Photo", "Seklin olcusu 3mb-den artiq olmamalidir.");
                        return View();
                    }
                string filename = await updateSlideVm.Photo.CreateFileAsync(_env.WebRootPath, "assets","images", "slider", "slide-img");
                exist.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider", "slide-img");
                exist.Image = filename;
            }
            
            exist.Title= updateSlideVm.Title;
            exist.subTitle = updateSlideVm.Title;
            exist.Desc = updateSlideVm.Desc;
            exist.Order = updateSlideVm.Order;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
