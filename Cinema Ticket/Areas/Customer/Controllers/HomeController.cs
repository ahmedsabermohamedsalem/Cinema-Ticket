using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _repository;

        public HomeController(IRepository<Movie> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }
    }
}