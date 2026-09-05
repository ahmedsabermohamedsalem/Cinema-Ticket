using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Utilities.DBSeeder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  ,{CD.EMPLOYEE_ROLE} ")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _repository;

        public CategoryController(IRepository<Category> repository)
        {
            _repository = repository;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _repository.GetAllAsync(
                includes: new[]
                {
                    (System.Linq.Expressions.Expression<Func<Category, object>>)
                        (c => c.Movies)
                },
                IsTraked: false);

            return View(categories);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category newCategory)
        {
            if (newCategory == null)
            {
                return View(newCategory);
            }

            await _repository.InsertAsync(newCategory);
            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _repository.GetOneAsync(
                c => c.Id == id,
                includes: new[]
                {
                    (System.Linq.Expressions.Expression<Func<Category, object>>)
                        (c => c.Movies)
                },
                IsTraked: false);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _repository.GetOneAsync(
                x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category newCategory)
        {
            if (newCategory == null)
            {
                return View(newCategory);
            }

            var category = await _repository.GetOneAsync(
                x => x.Id == newCategory.Id);

            if (category == null)
            {
                return NotFound();
            }

            category.Name = newCategory.Name;
            category.Description = newCategory.Description;

            _repository.Update(category);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _repository.GetOneAsync(
                x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _repository.Delete(category);

            await _repository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}