using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Cinema_Ticket.Utilities.DBSeeder;


namespace Ecommerce531.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE} , {CD.ADMIN_ROLE}  ,{CD.EMPLOYEE_ROLE} ")]


    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
        public async Task<IActionResult> LockUnLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            bool isSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN_ROLE);
            if (isSuperAdmin)
            {
                TempData["Error_Notification"] = "Can't not Lock Super Admin!!!1";
                return RedirectToAction(nameof(Index));

            }
            // UnLocked
            if (user.LockoutEnd == null || DateTime.UtcNow > user.LockoutEnd)
            {
                // Lock 
                await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(10));
            }
            else
            {
                // UnLocked
                await _userManager.SetLockoutEndDateAsync(user, null);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
