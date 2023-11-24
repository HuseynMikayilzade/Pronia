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
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (slide.Photo == null)
            {
                ModelState.AddModelError("Photo", "Sekil daxil edin");
                return View();
            }
            if (!slide.Photo.CheckType("image/"))  
            {
                ModelState.AddModelError("Photo", "faylin tipi uygun deyil");
                return View();
            }
            if (slide.Photo.CheckSize(3))
            {
                ModelState.AddModelError("Photo", "Seklin olcusu 3mb-den artiq olmamalidir.");
                return View();
            }




            slide.Image = await slide.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");

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

            exist.Image.DeleteFile(_env.WebRootPath, "uploads", "slide");
           
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

            return View(existed);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Slide slide)
        {
            Slide exist =await _context.Slides.FirstOrDefaultAsync(s=>s.Id == id);
            if (exist == null) return NotFound();
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (slide.Photo != null)
            {
                    if (!slide.Photo.CheckType("image/"))
                    {
                        ModelState.AddModelError("Photo", "faylin tipi uygun deyil");
                        return View();
                    }
                    if (slide.Photo.CheckSize(3))
                    {
                        ModelState.AddModelError("Photo", "Seklin olcusu 3mb-den artiq olmamalidir.");
                        return View();
                    }
                string filename = await slide.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");
                exist.Image.DeleteFile(_env.WebRootPath, "uploads", "slide");
                exist.Image = filename;
            }
            
            exist.Title= slide.Title;
            exist.subTitle = slide.Title;
            exist.Desc = slide.Desc;
            exist.Order = slide.Order;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
