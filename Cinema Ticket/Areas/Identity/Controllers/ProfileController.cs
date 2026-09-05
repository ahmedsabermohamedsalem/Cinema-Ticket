using Cinema_Ticket.Models;
using Cinema_Ticket.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var applicationUserVM = user.Adapt<ApplicationUserVM>();

            return View(applicationUserVM);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            user.Name = applicationUserVM.Name;
            user.address = applicationUserVM.Adderss;
            user.PhoneNumber = applicationUserVM.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    " , ",
                    result.Errors.Select(e => e.Description)
                );

                TempData["Error_Notification"] = errors;

                return RedirectToAction(nameof(Index));
            }

            TempData["Successful_Notification"] =
                "Profile updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                user,
                applicationUserVM.CurrentPassword,
                applicationUserVM.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    " , ",
                    result.Errors.Select(e => e.Description)
                );

                TempData["Error_Notification"] = errors;

                return RedirectToAction(nameof(Index));
            }

            TempData["Successful_Notification"] =
                "Password changed successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}