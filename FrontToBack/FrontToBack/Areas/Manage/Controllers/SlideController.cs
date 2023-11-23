using FrontToBack.DAL;
using FrontToBack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;

        public SlideController(AppDbContext context)
        {
            _context = context;
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
            if (!slide.Photo.ContentType.Contains("image/"))  
            {
                ModelState.AddModelError("Photo", "faylin tipi uygun deyil");
                return View();
            }
            if (slide.Photo.Length > 3 * 1024 * 1024)
            {
                ModelState.AddModelError("Photo", "Seklin olcusu 3mb-den artiq olmamalidir.");
                return View();
            }

            FileStream stream = new FileStream(@"C:\Users\User\Desktop\task\FrontToBack\FrontToBack\wwwroot\uploads\slide\" + slide.Photo.FileName,FileMode.Create);
           
            await slide.Photo.CopyToAsync(stream);
            slide.Image = slide.Photo.FileName;

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return NotFound();


            Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);

            if (existed == null) return NotFound();

            return View(existed);


    }   }
}
