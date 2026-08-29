using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IRepository<Cinema> _repository;

        public CinemaController(IRepository<Cinema> repository)
        {
            _repository = repository;
        }

        // GET: Admin/Cinema
        public async Task<IActionResult> Index()
        {
            var cinemas = await _repository.GetAllAsync(
                IsTraked: false);

            return View(cinemas);
        }

        // GET: Admin/Cinema/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Cinema/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Cinema cinema,
            IFormFile? ImageFile)
        {
            if (cinema == null)
            {
                return View(cinema);
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                cinema.Img = Imag.Createfile(ImageFile);
            }

            await _repository.InsertAsync(cinema);
            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Cinema/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cinema = await _repository.GetOneAsync(
                c => c.Id == id,
                IsTraked: false);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // GET: Admin/Cinema/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cinema = await _repository.GetOneAsync(
                c => c.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }

        // POST: Admin/Cinema/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Cinema cinema,
            IFormFile? ImageFile)
        {
            if (cinema == null)
            {
                return View(cinema);
            }

            var oldCinema = await _repository.GetOneAsync(
                c => c.Id == cinema.Id);

            if (oldCinema == null)
            {
                return NotFound();
            }

            oldCinema.Name = cinema.Name;
            oldCinema.Description = cinema.Description;

            // صورة جديدة
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // حذف الصورة القديمة
                if (!string.IsNullOrEmpty(oldCinema.Img))
                {
                    Imag.deletefile(oldCinema.Img);
                }

                // حفظ الصورة الجديدة
                oldCinema.Img = Imag.Createfile(ImageFile);
            }

            _repository.Update(oldCinema);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Cinema/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await _repository.GetOneAsync(
                c => c.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            // حذف صورة السينما
            if (!string.IsNullOrEmpty(cinema.Img))
            {
                Imag.deletefile(cinema.Img);
            }

            _repository.Delete(cinema);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}