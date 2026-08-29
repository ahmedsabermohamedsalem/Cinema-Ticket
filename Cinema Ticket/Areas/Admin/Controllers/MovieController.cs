using System.Linq.Expressions;
using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.services;
using Cinema_Ticket.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<MovieActor> _movieActorRepository;
        private readonly IRepository<MovieImage> _movieImageRepository;

        public MovieController(
            IRepository<Movie> movieRepository,
            IRepository<Actor> actorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<MovieActor> movieActorRepository,
            IRepository<MovieImage> movieImageRepository)
        {
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _movieActorRepository = movieActorRepository;
            _movieImageRepository = movieImageRepository;
        }

        // =========================================================
        // Index
        // =========================================================

        public async Task<IActionResult> Index(
            string? search,
            int? actorId,
            int? categoryId,
            int? cinemaId,
            int page = 1)
        {
            int pageSize = 5;

            Expression<Func<Movie, object>>[] includes =
            {
                x => x.Category,
                x => x.Cinema,
                x => x.MovieActors,
                x => x.MovieImages
            };

            var movies = await _movieRepository.GetAllAsync(
                includes: includes,
                IsTraked: false);

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                movies = movies.Where(x =>
                    x.Name.Contains(search,
                        StringComparison.OrdinalIgnoreCase));
            }

            // Category
            if (categoryId.HasValue)
            {
                movies = movies.Where(x =>
                    x.CategoryId == categoryId.Value);
            }

            // Cinema
            if (cinemaId.HasValue)
            {
                movies = movies.Where(x =>
                    x.CinemaId == cinemaId.Value);
            }

            // Actor
            if (actorId.HasValue)
            {
                movies = movies.Where(x =>
                    x.MovieActors.Any(ma =>
                        ma.ActorId == actorId.Value));
            }

            var movieList = movies
                .OrderByDescending(x => x.Id)
                .ToList();

            int totalMovies = movieList.Count;

            int totalPages = (int)Math.Ceiling(
                totalMovies / (double)pageSize);

            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            movieList = movieList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Filters
            ViewBag.Actors = await _actorRepository.GetAllAsync(
                IsTraked: false);

            ViewBag.Categories = await _categoryRepository.GetAllAsync(
                IsTraked: false);

            ViewBag.Cinemas = await _cinemaRepository.GetAllAsync(
                IsTraked: false);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            ViewBag.Search = search;
            ViewBag.ActorId = actorId;
            ViewBag.CategoryId = categoryId;
            ViewBag.CinemaId = cinemaId;

            return View(movieList);
        }

        // =========================================================
        // Create GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadMovieData();

            return View();
        }

        // =========================================================
        // Create POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(movieModelVM vm)
        {
            if (vm == null || vm.Movie == null)
            {
                await LoadMovieData();
                return View(vm);
            }

            // Main Image
            if (vm.ImageFile != null &&
                vm.ImageFile.Length > 0)
            {
                vm.Movie.MainImg =
                    Imag.Createfile(vm.ImageFile);
            }

            // Save Movie
            await _movieRepository.InsertAsync(vm.Movie);
            await _movieRepository.CommitAsync();

            int movieId = vm.Movie.Id;

            // Actors
            if (vm.ActorIds != null)
            {
                foreach (var actorId in vm.ActorIds)
                {
                    MovieActor movieActor = new MovieActor
                    {
                        MovieId = movieId,
                        ActorId = actorId
                    };

                    await _movieActorRepository
                        .InsertAsync(movieActor);
                }
            }

            // Sub Images
            if (vm.SubImages != null)
            {
                foreach (var image in vm.SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        MovieImage movieImage = new MovieImage
                        {
                            MovieId = movieId,
                            Img = Imag.Createfile(image)
                        };

                        await _movieImageRepository
                            .InsertAsync(movieImage);
                    }
                }
            }

            await _movieActorRepository.CommitAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = movieId });
        }

        // =========================================================
        // Details
        // =========================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Expression<Func<Movie, object>>[] includes =
            {
                x => x.Category,
                x => x.Cinema,
                x => x.MovieActors,
                x => x.MovieImages
            };

            var movie = await _movieRepository.GetOneAsync(
                x => x.Id == id,
                includes,
                false);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // =========================================================
        // Edit GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            Expression<Func<Movie, object>>[] includes =
            {
                x => x.MovieActors,
                x => x.MovieImages
            };

            var movie = await _movieRepository.GetOneAsync(
                x => x.Id == id,
                includes);

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

            await LoadMovieData(
                movie.CategoryId,
                movie.CinemaId,
                actorIds);

            return View(vm);
        }

        // =========================================================
        // Edit POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(movieModelVM vm)
        {
            if (vm == null || vm.Movie == null)
            {
                return NotFound();
            }

            Expression<Func<Movie, object>>[] includes =
            {
                x => x.MovieActors,
                x => x.MovieImages
            };

            var oldMovie = await _movieRepository.GetOneAsync(
                x => x.Id == vm.Movie.Id,
                includes);

            if (oldMovie == null)
            {
                return NotFound();
            }

            // Movie Data
            oldMovie.Name = vm.Movie.Name;
            oldMovie.Description = vm.Movie.Description;
            oldMovie.Price = vm.Movie.Price;
            oldMovie.Duration = vm.Movie.Duration;
            oldMovie.ReleaseDate = vm.Movie.ReleaseDate;
            oldMovie.CategoryId = vm.Movie.CategoryId;
            oldMovie.CinemaId = vm.Movie.CinemaId;

            // Main Image
            if (vm.ImageFile != null &&
                vm.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(oldMovie.MainImg))
                {
                    Imag.deletefile(oldMovie.MainImg);
                }

                oldMovie.MainImg =
                    Imag.Createfile(vm.ImageFile);
            }

            _movieRepository.Update(oldMovie);

            // =====================================================
            // Actors
            // =====================================================

            foreach (var oldActor in oldMovie.MovieActors.ToList())
            {
                _movieActorRepository.Delete(oldActor);
            }

            await _movieActorRepository.CommitAsync();

            if (vm.ActorIds != null)
            {
                foreach (var actorId in vm.ActorIds)
                {
                    MovieActor movieActor = new MovieActor
                    {
                        MovieId = oldMovie.Id,
                        ActorId = actorId
                    };

                    await _movieActorRepository
                        .InsertAsync(movieActor);
                }
            }

            // =====================================================
            // New Sub Images
            // =====================================================

            if (vm.SubImages != null)
            {
                foreach (var image in vm.SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        MovieImage movieImage = new MovieImage
                        {
                            MovieId = oldMovie.Id,
                            Img = Imag.Createfile(image)
                        };

                        await _movieImageRepository
                            .InsertAsync(movieImage);
                    }
                }
            }

            await _movieRepository.CommitAsync();

            return RedirectToAction(
                nameof(Details),
                new { id = oldMovie.Id });
        }

        // =========================================================
        // Delete Image
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _movieImageRepository.GetOneAsync(
                x => x.Id == id);

            if (image == null)
            {
                return NotFound();
            }

            int movieId = image.MovieId;

            if (!string.IsNullOrEmpty(image.Img))
            {
                Imag.deletefile(image.Img);
            }

            _movieImageRepository.Delete(image);

            await _movieImageRepository.CommitAsync();

            return RedirectToAction(
                nameof(Edit),
                new { id = movieId });
        }

        // =========================================================
        // Delete Movie
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Expression<Func<Movie, object>>[] includes =
            {
                x => x.MovieActors,
                x => x.MovieImages
            };

            var movie = await _movieRepository.GetOneAsync(
                x => x.Id == id,
                includes);

            if (movie == null)
            {
                return NotFound();
            }

            // Delete Main Image
            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                Imag.deletefile(movie.MainImg);
            }

            // Delete Sub Images
            foreach (var image in movie.MovieImages.ToList())
            {
                if (!string.IsNullOrEmpty(image.Img))
                {
                    Imag.deletefile(image.Img);
                }

                _movieImageRepository.Delete(image);
            }

            // Delete Actors
            foreach (var actor in movie.MovieActors.ToList())
            {
                _movieActorRepository.Delete(actor);
            }

            await _movieImageRepository.CommitAsync();

            await _movieActorRepository.CommitAsync();

            // Delete Movie
            _movieRepository.Delete(movie);

            await _movieRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // Helper
        // =========================================================

        private async Task LoadMovieData(
            int? categoryId = null,
            int? cinemaId = null,
            List<int>? actorIds = null)
        {
            var categories =
                await _categoryRepository.GetAllAsync(
                    IsTraked: false);

            var cinemas =
                await _cinemaRepository.GetAllAsync(
                    IsTraked: false);

            var actors =
                await _actorRepository.GetAllAsync(
                    IsTraked: false);

            ViewBag.Categories = new SelectList(
                categories,
                "Id",
                "Name",
                categoryId);

            ViewBag.Cinemas = new SelectList(
                cinemas,
                "Id",
                "Name",
                cinemaId);

            ViewBag.Actors = actors
                .Select(a => new SelectListItem
                {
                    Value = a.ID.ToString(),
                    Text = a.Name,
                    Selected =
                        actorIds != null &&
                        actorIds.Contains(a.ID)
                })
                .ToList();
        }
    }
}