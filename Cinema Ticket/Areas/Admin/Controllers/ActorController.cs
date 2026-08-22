using System.Diagnostics.Eventing.Reader;
using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Cinema_Ticket.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.Areas.Admin.Controllers
{


    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDBcContext _context = new ApplicationDBcContext();

        public IActionResult Index()
        {




            return View(_context.actors.ToList());

        }


        public IActionResult Create()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile Imagefile)

        {

            if (actor != null && Imagefile.Length > 0)
            {

                actor.Img = Imag.Createfile(Imagefile);
                //return View(cinema);

                _context.actors.Add(actor);
                _context.SaveChanges();

                return RedirectToAction("Index");

            }

            return View(actor);


        }








        public IActionResult Edit(int id)

        {

            var actor = _context.actors.FirstOrDefault(x => x.ID == id);


            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile ImageFile)
        {
            var oldActor = _context.actors
                .AsNoTracking()
                .FirstOrDefault(x => x.ID == actor.ID);

            if (actor != null)
            {
                if (oldActor == null)
                {
                    return NotFound();
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    Imag.deletefile(oldActor.Img);

                    actor.Img = Imag.Createfile(ImageFile);
                }
                else
                {
                    // لو مفيش صورة جديدة، احتفظ بالصورة القديمة
                    actor.Img = oldActor.Img;
                }

                _context.actors.Update(actor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(actor);
        }



        public IActionResult Details(int id)
        {
            return View(_context.actors.FirstOrDefault(a => a.ID == id));
        }








        public IActionResult Delete(int id)
        {


            var Actor = _context.actors.Find(id);
            _context.actors.Remove(Actor);
            _context.SaveChanges();



            return RedirectToAction("Index");
        }
    }
}