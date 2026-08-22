using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly ApplicationDBcContext _dbContext = new ApplicationDBcContext();

        public IActionResult Index()
        {



            return View(_dbContext.categories.Include(c=>c.Movies).ToList());
        }


        public IActionResult Create()

        {
            return View(); 
        }

        [HttpPost]
        public IActionResult Create(Category newCategory )

        {

            if (newCategory != null)
            {

                _dbContext.categories.Add(newCategory);
                _dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }



        public IActionResult Details(int id)
        {



            return View(_dbContext.categories.FirstOrDefault(c => c.Id == id));
        
        }







        public IActionResult Edit(int id)
        {
            var category = _dbContext.categories
                .FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Edit(Category Newcategory)
        {
            var category = _dbContext.categories
                .FirstOrDefault(x => x.Id == Newcategory.Id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = Newcategory.Name;
            category.Description = Newcategory.Description;

            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Delete(int id)
        {



            Category Ncategory = _dbContext.categories.Find(id);
            _dbContext.categories.Remove(Ncategory);
            _dbContext.SaveChanges();

            return  RedirectToAction("index");
        }



    }
}

