using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Cinema_Ticket.services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDBcContext _context = new ApplicationDBcContext();

        
        public IActionResult Index()
        {
            var cinemas = _context.cinemas.ToList();

            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            var cinema = _context.cinemas.FirstOrDefault(c => c.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            return View(cinema);
        }


        [HttpPost]

        public IActionResult Create(Cinema cinema, IFormFile ImageFile)
        {
            if (cinema != null && ImageFile.Length > 0)
            {

                cinema.Img = Imag.Createfile(ImageFile);
                //return View(cinema);

                _context.cinemas.Add(cinema);
                _context.SaveChanges();

                return RedirectToAction("Index");

                          }



            return View(cinema);
        }

        public IActionResult Edit( int id)
        {




            return View(_context.cinemas.FirstOrDefault(x=> x.Id==id));
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile ImageFile)
        {
            var oldCinema = _context.cinemas.Find(cinema.Id);

            if (oldCinema == null)
            {
                return NotFound();
            }

            oldCinema.Name = cinema.Name;
            oldCinema.Description = cinema.Description;

            // If user uploaded a new image
            if (ImageFile != null)
            {
                // Save the old image name BEFORE changing it
                var oldImageName = oldCinema.Img;

                // Delete old image
                if (!string.IsNullOrEmpty(oldImageName))
                {
                    var oldImagePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploadsFile",
                        oldImageName
                    );

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                oldCinema.Img = Imag.Createfile(ImageFile);
            }

            _context.cinemas.Update(oldCinema);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {


            var cinema = _context.cinemas.Find(id);
            _context.cinemas.Remove(cinema);
            _context.SaveChanges();



            return RedirectToAction("Index");
        }

    }

           








         

    
   

}

