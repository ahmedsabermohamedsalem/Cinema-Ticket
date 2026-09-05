using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Utilities.DBSeeder;
using Cinema_Ticket.Viewmodel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  ,{CD.EMPLOYEE_ROLE} ")]
    public class HomeController : Controller
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Movie> _movieRepository;

        public HomeController(
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Actor> actorRepository,
            IRepository<Movie> movieRepository)
        {
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync(
                IsTraked: false);

            var cinemas = await _cinemaRepository.GetAllAsync(
                IsTraked: false);

            var actors = await _actorRepository.GetAllAsync(
                IsTraked: false);

            var movies = await _movieRepository.GetAllAsync(
                IsTraked: false);

            DashBordVm vm = new DashBordVm()
            {
                CountCategory = categories.Count(),
                CountCinema = cinemas.Count(),
                CountActoor = actors.Count(),
                CountMovies = movies.Count()
            };

            return View(vm);
        }
    }
}