using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Utilities.DBSeeder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]

    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  ,{CD.EMPLOYEE_ROLE} ")]
    public class ShowController : Controller
    {
        private readonly IRepository<Show> _show;

        public ShowController(IRepository<Show> show)
        {
            _show = show;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _show.GetAllAsync());
        }


        public IActionResult create()
        { return View(); }

        [HttpPost]
        public async Task<IActionResult> create(Show newhall)
        {
            if (ModelState != null)
            {

                await _show.InsertAsync(newhall);
                await _show.CommitAsync();
                return RedirectToAction("Index");


            }

            return View(newhall);

        }


        public IActionResult Edit(int? id)
        {

            var hall = _show.GetOneAsync(h => h.Id == id);




            return View(hall);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Show newhall)
        {

            if (ModelState != null)
            {
                _show.Update(newhall);
                await _show.CommitAsync();




            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {

            var nhall = await _show.GetOneAsync(h => h.Id == id);



            _show.Delete(nhall);
            await _show.CommitAsync();
            return RedirectToAction("index");
        }



    }
}
