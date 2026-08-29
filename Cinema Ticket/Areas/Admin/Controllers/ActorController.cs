using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _repository;

        public ActorController(IRepository<Actor> repository)
        {
            _repository = repository;
        }

        // GET: Admin/Actor
        public async Task<IActionResult> Index()
        {
            var actors = await _repository.GetAllAsync(
                IsTraked: false);

            return View(actors);
        }

        // GET: Admin/Actor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Actor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Actor actor,
            IFormFile? Imagefile)
        {
            if (actor == null)
            {
                return View(actor);
            }

            if (Imagefile != null && Imagefile.Length > 0)
            {
                actor.Img = Imag.Createfile(Imagefile);
            }

            await _repository.InsertAsync(actor);
            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Actor/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _repository.GetOneAsync(
                x => x.ID == id);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Admin/Actor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Actor actor,
            IFormFile? ImageFile)
        {
            if (actor == null)
            {
                return View(actor);
            }

            var oldActor = await _repository.GetOneAsync(
                x => x.ID == actor.ID);

            if (oldActor == null)
            {
                return NotFound();
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(oldActor.Img))
                {
                    Imag.deletefile(oldActor.Img);
                }

                actor.Img = Imag.Createfile(ImageFile);
            }
            else
            {
                actor.Img = oldActor.Img;
            }

            _repository.Update(actor);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Actor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var actor = await _repository.GetOneAsync(
                x => x.ID == id,
                IsTraked: false);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Admin/Actor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await _repository.GetOneAsync(
                x => x.ID == id);

            if (actor == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(actor.Img))
            {
                Imag.deletefile(actor.Img);
            }

            _repository.Delete(actor);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}