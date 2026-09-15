using System.Security.Claims;
using EmpLMS.Data.Repositories;
using EmpLMS.Helpers;
using EmpLMS.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmpLMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepo;

        public AccountController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to respective dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Dashboard", "Admin");

                return RedirectToAction("Dashboard", "Employee");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Find user by email
                var user = await _authRepo.ValidateUserLoginAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email address or password.");
                    return View(model);
                }

                // Check if user is active
                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account has been deactivated. Please contact the administrator.");
                    return View(model);
                }

                // Check password
                if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Invalid email address or password.");
                    return View(model);
                }

                // Set up claims for authentication cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Name, user.FullName)
                };

                if (user.EmployeeId.HasValue)
                {
                    claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
                    claims.Add(new Claim("EmployeeCode", user.EmployeeCode ?? ""));
                    claims.Add(new Claim("DepartmentName", user.DepartmentName ?? ""));
                    claims.Add(new Claim("Designation", user.Designation ?? ""));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false
                };

                // Sign in using cookie authentication
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Determine redirection URL
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                return RedirectToAction("Dashboard", "Employee");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        // GET or POST: /Account/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

