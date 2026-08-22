using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Viewmodel;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDBcContext _context = new ApplicationDBcContext();


        [Area("Admin")]
        public IActionResult Index()
        {

            DashBordVm vm = new DashBordVm()
            {
                CountCategory = _context.categories.Count(),
                CountCinema = _context.cinemas.Count(),
                CountActoor = _context.actors.Count(),
                CountMovies = _context.Movies.Count()



            };

            return View(vm);
        }
    }
}
