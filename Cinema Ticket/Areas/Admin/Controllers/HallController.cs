
using System.Linq.Expressions;
using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Utilities.DBSeeder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE},{CD.EMPLOYEE_ROLE}")]
    public class HallController : Controller
    {
        private readonly IRepository<Hall> _hall;
        private readonly IRepository<Cinema> _cinema;

        public HallController(
            IRepository<Hall> hall,
            IRepository<Cinema> cinema)
        {
            _hall = hall;
            _cinema = cinema;
        }

        // GET: Admin/Hall
        public async Task<IActionResult> Index()
        {
            var halls = await _hall.GetAllAsync(
       includes: [h => h.Cinema]
   );


            return View(halls);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Cinemas = await _cinema.GetAllAsync();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hall newHall)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        Console.WriteLine(
                            $"{item.Key} : {error.ErrorMessage}"
                        );
                    }
                }
            }

            if (ModelState.IsValid)
            {
                await _hall.InsertAsync(newHall);
                await _hall.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cinemas = await _cinema.GetAllAsync();

            return View(newHall);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Hall newHall)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _hall.InsertAsync(newHall);
        //        await _hall.CommitAsync();

        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewBag.Cinemas = await _cinema.GetAllAsync();

        //    return View(newHall);
        //}


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var hall = await _hall.GetOneAsync(h => h.Id == id);

            if (hall == null)
                return NotFound();

            ViewBag.Cinemas = await _cinema.GetAllAsync();

            return View(hall);
        }

        // POST: Admin/Hall/Edit
        [HttpPost]
       
        public async Task<IActionResult> Edit(Hall newHall)
        {
            if (ModelState.IsValid)
            {
                _hall.Update(newHall);
                await _hall.CommitAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cinemas = await _cinema.GetAllAsync();

            return View(newHall);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var hall = await _hall.GetOneAsync(h => h.Id == id);

            if (hall == null)
                return NotFound();

            _hall.Delete(hall);
            await _hall.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

