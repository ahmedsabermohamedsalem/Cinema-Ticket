
using System.Security.Claims;
using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Viewmodel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema_Ticket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TicketController : Controller
    {
        private readonly IRepository<Movie> _movie;
        private readonly IRepository<Hall> _hall;
        private readonly IRepository<Show> _show;
        private readonly IRepository<Cart> _cart;

        public TicketController(
            IRepository<Movie> movie,
            IRepository<Hall> hall,
            IRepository<Show> show,
            IRepository<Cart> cart)
        {
            _movie = movie;
            _hall = hall;
            _show = show;
            _cart = cart;
        }

        // GET: Customer/Ticket/Create
        [HttpGet]
        public async Task<IActionResult> Create(int movieId)
        {
            var movie = await _movie.GetOneAsync(
                m => m.Id == movieId
            );

            if (movie == null)
                return NotFound();

            var halls = await _hall.GetAllAsync();
            var shows = await _show.GetAllAsync();

            var vm = new CreateTicketVM
            {
                MovieId = movie.Id,
                Movie = movie,

                Halls = halls.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                }),

                Shows = shows.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.StartDate:dd/MM/yyyy hh:mm tt} - {s.EndDate:hh:mm tt}"
                })
            };

            return View(vm);
        }

        // POST: Customer/Ticket/Create
        [HttpPost]
        
        public async Task<IActionResult> Create(CreateTicketVM vm)
        {
            if (!ModelState.IsValid)
            {
                var halls = await _hall.GetAllAsync();
                var shows = await _show.GetAllAsync();

                vm.Halls = halls.Select(h => new SelectListItem
                {
                    Value = h.Id.ToString(),
                    Text = h.Name
                });

                vm.Shows = shows.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.StartDate:dd/MM/yyyy hh:mm tt} - {s.EndDate:hh:mm tt}"
                });

                vm.Movie = await _movie.GetOneAsync(
                    m => m.Id == vm.MovieId
                );

                return View(vm);
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (userId == null)
                return Unauthorized();

            // Get Movie from Database
            var movie = await _movie.GetOneAsync(
                m => m.Id == vm.MovieId
            );

            if (movie == null)
                return NotFound();

            // Create Cart
            var cart = new Cart
            {
                UserId = userId,
                MovieId = movie.Id,
                HallId = vm.HallId,
                ShowId = vm.ShowId,

                // Price ALWAYS comes from Movie
                Price = movie.Price
            };

            await _cart.InsertAsync(cart);
            await _cart.CommitAsync();

            return RedirectToAction(
                "Index",
                "Cart",
                new { area = "Customer" }
            );
        }
    }
}
