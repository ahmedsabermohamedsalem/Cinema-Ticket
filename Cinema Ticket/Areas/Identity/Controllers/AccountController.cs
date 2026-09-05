
using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.Viewmodel;
using Ecommerce531.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;


        private readonly IEmailSender _emailSender;
        

        public AccountController(
            UserManager<ApplicationUser> usermanger,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _usermanger = usermanger;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // =========================
        // Register GET
        // =========================
        public IActionResult Register()
        {
            return View();
        }


        // =========================
        // Register POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVm);
            }

            var user = new ApplicationUser()
            {
                Name = registerVm.Name,
                address = registerVm.Address,
                Email = registerVm.Email,
                UserName = registerVm.UserName
            };

            var result = await _usermanger.CreateAsync(
                user,
                registerVm.Password
            );

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(registerVm);
            }

            // Generate Email Confirmation Token
            var token = await _usermanger.GenerateEmailConfirmationTokenAsync(user);

            // Generate Confirmation Link
            var link = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    area = "Identity",
                    userId = user.Id,
                    token = token
                },
                Request.Scheme
            );

            // Send Email
            await _emailSender.SendEmailAsync(
                registerVm.Email,
                "Ecommerce531 Confirm Email",
                $"<h1>Please click <a href=\"{link}\">here</a> to confirm your email.</h1>"
            );

            TempData["Successful_Notification"] =
                "User Created Successfully. Please check your email to confirm your account.";

            return RedirectToAction(nameof(Login));
        }


        // =========================
        // Confirm Email
        // =========================
        public async Task<IActionResult> ConfirmEmail(
            string userId,
            string token)
        {
            var user = await _usermanger.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _usermanger.ConfirmEmailAsync(
                user,
                token
            );

            if (!result.Succeeded)
            {
                TempData["Error_Notification"] =
                    "Problem happened while confirming your email.";

                return RedirectToAction(nameof(Login));
            }

            TempData["Successful_Notification"] =
                "Email Confirmation Successfully.";

            return RedirectToAction(nameof(Login));
        }


        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVm resendEmailConfirmationVM)
        {
            var user = await _usermanger.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
                        await _usermanger.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid userName or Email");
                return View(resendEmailConfirmationVM);
            }
            var token = await _usermanger.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ConfirmEmail", "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce531 Confirm Email",
                $"<h1> please click <a href={link}>here<a/> to Confirm You Email  </h1>");
            return RedirectToAction(nameof(Login));
        }




        // =========================
        // Login GET
        // =========================
        public IActionResult Login()
        {
            return View();
        }


        // =========================
        // Login POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            // Search by Email first
            var user = await _usermanger.FindByEmailAsync(
                loginVM.UserNameOrEmail
            );

            // If not found, search by Username
            if (user == null)
            {
                user = await _usermanger.FindByNameAsync(
                    loginVM.UserNameOrEmail
                );
            }

            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid username or password."
                );

                return View(loginVM);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginVM.Password,
                loginVM.RememberMe,
                lockoutOnFailure: true
            );

            // Login Success
            if (result.Succeeded)
            {
                TempData["Successful_Notification"] =
                    "Login Successfully.";

                return RedirectToAction(
                    "Index",
                    "Home",
                    new { area = "Customer" }
                );
            }

            // Account Locked
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    "",
                    "Too many failed attempts. Please try again later."
                );

                return View(loginVM);
            }

            // Email Not Confirmed
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(
                    "",
                    "Please confirm your email first."
                );

                return View(loginVM);
            }

            // Invalid Password
            ModelState.AddModelError(
                "",
                "Invalid username or password."
            );

            return View(loginVM);
        }


        public IActionResult ForgetPassword()
        {
            return View();
        }

        public IActionResult VerifyOtp()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtpVM verifyOtpVM)
        {
            var user = await _usermanger.FindByIdAsync(verifyOtpVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid user");
                return View(verifyOtpVM);
            }
            var otps = await _applicationUserOtpRepository.GetAllAsync(o =>
                o.ApplicationUserId == user.Id &&
                o.IsValid == true &&
                o.ValidTo >= DateTime.UtcNow
                );
            var applicationUserotp = otps.OrderByDescending(o => o.CreatedAt).FirstOrDefault();
            if (applicationUserotp == null || applicationUserotp.OTP != verifyOtpVM.OTP)
            {
                ModelState.AddModelError("", "invalid / Expired  otp ");
                return View(verifyOtpVM);
            }
            applicationUserotp.IsValid = false;
            await _applicationUserOtpRepository.CommitAsync();
            var token = await _usermanger.GeneratePasswordResetTokenAsync(user);

            return RedirectToAction(nameof(ResetPassword), new { userId = user.Id, token });
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVm forgetPasswordVM)
        {
            var user = await _usermanger.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail) ??
                await _usermanger.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid userName");
                return View(forgetPasswordVM);
            }
            var otps = await _applicationUserOtpRepository.GetAllAsync(o => o.ApplicationUserId == user.Id);
            var count = otps.Count(o => (DateTime.UtcNow - o.CreatedAt).TotalHours <= 24);
            if (count > 5)
            {
                ModelState.AddModelError("", "Too Many attempts Please Try Again Later");
                return View(forgetPasswordVM);
            }
            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(otp, user.Id);
            await _applicationUserOtpRepository.InsertAsync(applicationUserOtp);
            await _applicationUserOtpRepository.CommitAsync();
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce531 Reset Password",
                $"<h1> use This OTP {otp}to Reset Your Password </h1>");
            return RedirectToAction(nameof(VerifyOtp), new { userId = user.Id });
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordVM { UserId = userId, Token = token });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPasswordVM);
            }
            var user = await _usermanger.FindByIdAsync(resetPasswordVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid user ");
                return View(resetPasswordVM);
            }
            var result = await _usermanger.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetPasswordVM);
            }
            TempData["Successful_Notification"] = "Reset Password Successfully";
            return RedirectToAction(nameof(Login));

        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }




    }




}