using System.Security.Cryptography.X509Certificates;
using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Cinema_Ticket.services;
using Cinema_Ticket.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {

        private readonly ApplicationDBcContext _context = new ApplicationDBcContext( );

        public IActionResult Index(
      string search,
      int? actorId,
      int? categoryId,
      int? cinemaId,
      int page = 1)
        {
            int pageSize = 5;

            var movies = _context.Movies
                .Include(x => x.Category)
                .Include(x => x.Cinema)
                .Include(x => x.MovieActors)
                    .ThenInclude(x => x.Actor)
                .Include(x => x.MovieImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                movies = movies.Where(x => x.Name.Contains(search));
            }

            if (categoryId != null)
            {
                movies = movies.Where(x => x.CategoryId == categoryId);
            }

            if (cinemaId != null)
            {
                movies = movies.Where(x => x.CinemaId == cinemaId);
            }

            if (actorId != null)
            {
                movies = movies.Where(x =>
                    x.MovieActors.Any(ma => ma.ActorId == actorId));
            }

            int totalMovies = movies.Count();

            int totalPages = (int)Math.Ceiling(
                totalMovies / (double)pageSize);

            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            movies = movies
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            ViewBag.Actors = _context.actors.ToList();
            ViewBag.Categories = _context.categories.ToList();
            ViewBag.Cinemas = _context.cinemas.ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewBag.Search = search;
            ViewBag.ActorId = actorId;
            ViewBag.CategoryId = categoryId;
            ViewBag.CinemaId = cinemaId;

            return View(movies.ToList());
        }






        //    return View(movies);
        //}


        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(x => x.MovieActors)
                .Include(x => x.MovieImages)
                .FirstOrDefault(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var actorIds = movie.MovieActors
                .Select(x => x.ActorId)
                .ToList();

            var vm = new movieModelVM
            {
                Movie = movie,
                ActorIds = actorIds
            };


            // Categories

            ViewBag.Categories = new SelectList(
                _context.categories.ToList(),
                "Id",
                "Name",
                movie.CategoryId
            );


            // Cinemas

            ViewBag.Cinemas = new SelectList(
                _context.cinemas.ToList(),
                "Id",
                "Name",
                movie.CinemaId
            );


            // Actors
            // مهم: لا تستخدم MultiSelectList

            ViewBag.Actors = _context.actors
                .Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text = a.Name,
                    Selected = actorIds.Contains(a.ID)
                })
                .ToList();


            return View(vm);
        }



        [HttpPost]
        public IActionResult Edit(movieModelVM vm)
        {
            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction(
            //        "Edit",
            //        "Movie",
            //        new { id = vm.Movie.Id }
            //    );
            //}

            if (vm.Movie == null)
            {
                return NotFound();
            }


            // Get old movie
            var oldMovie = _context.Movies
                .Include(x => x.MovieImages)
                .Include(x => x.MovieActors)
                .FirstOrDefault(x => x.Id == vm.Movie.Id);


            if (oldMovie == null)
            {
                return NotFound();
            }


            // =========================
            // Movie Data
            // =========================

            oldMovie.Name = vm.Movie.Name;
            oldMovie.Description = vm.Movie.Description;
            oldMovie.Price = vm.Movie.Price;
            oldMovie.Duration = vm.Movie.Duration;
            oldMovie.ReleaseDate = vm.Movie.ReleaseDate;
            oldMovie.CategoryId = vm.Movie.CategoryId;
            oldMovie.CinemaId = vm.Movie.CinemaId;


            // =========================
            // Main Image
            // =========================

            if (vm.ImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(oldMovie.MainImg))
                {
                    Imag.deletefile(oldMovie.MainImg);
                }

                // Create new image
                oldMovie.MainImg = Imag.Createfile(vm.ImageFile);
            }


            // =========================
            // Actors
            // =========================

            _context.movieActors.RemoveRange(oldMovie.MovieActors);


            if (vm.ActorIds != null)
            {
                foreach (var actorId in vm.ActorIds)
                {
                    MovieActor movieActor = new MovieActor()
                    {
                        MovieId = oldMovie.Id,
                        ActorId = actorId
                    };

                    _context.movieActors.Add(movieActor);
                }
            }


            // =========================
            // Sub Images
            // =========================

            if (vm.SubImages != null)
            {
                foreach (var image in vm.SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        MovieImage movieImage = new MovieImage()
                        {
                            MovieId = oldMovie.Id,

                            Img = Imag.Createfile(image)
                        };

                        _context.MovieImages.Add(movieImage);
                    }
                }
            }


            // =========================
            // Save
            // =========================

            _context.SaveChanges();


            return RedirectToAction(
                "Details",
                "Movie",
                new { id = oldMovie.Id }
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteImage(int id)
        {
            var image = _context.MovieImages
                .FirstOrDefault(x => x.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            // نحفظ MovieId قبل حذف الصورة
            int movieId = image.MovieId;

            // حذف الملف من wwwroot/uploadsFile
            if (!string.IsNullOrEmpty(image.Img))
            {
                Imag.deletefile(image.Img);
            }

            // حذف الصورة من Database
            _context.MovieImages.Remove(image);

            _context.SaveChanges();

            // الرجوع إلى Edit
            return RedirectToAction(
                "Edit",
                "Movie",
                new { id = movieId }
            );
        }

        public IActionResult Create()
        {


            ViewBag.Categories = new SelectList(_context.categories, "Id", "Name");
            ViewBag.Cinemas = new SelectList(_context.cinemas, "Id", "Name");

            ViewBag.Actors = new SelectList(
                _context.actors.ToList(),
                "ID",
                "Name"
            );

            return View();
        }



        [HttpPost]
        public IActionResult create( movieModelVM vM)
        {



            if (vM != null)
            {
                Movie movie = vM.Movie;
                /// mainimg 
                /// 
                movie.MainImg = Imag.Createfile(vM.ImageFile);

                _context.Movies.Add(movie);
                _context.SaveChanges();
                int movid = movie.Id;



                foreach (var image in vM.SubImages)
                {

                    string path = Imag.Createfile(image);
                    MovieImage movieImage = new
                            MovieImage()
                    {
                        MovieId = movid,
                        Img = path
                    

                    };

                    _context.MovieImages.Add(movieImage);
                }

                // Add Actors
                foreach (var actor in vM.ActorIds)
                {
                    MovieActor movieActor = new MovieActor()
                    {
                        MovieId = movid,
                        ActorId = actor
                    };

                    _context.movieActors.Add(movieActor);
                }
                _context.SaveChanges();
               
















            }








            return View();
 
        }




        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .Include(m => m.MovieImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

    }
}
 