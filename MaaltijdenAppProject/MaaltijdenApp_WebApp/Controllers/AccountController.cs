using MaaltijdenApp_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Controllers
{
    /// <summary>
    /// This controller manages the authentication of users.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly SignInManager<IdentityUser> _signInMgr;

        public AccountController(
            UserManager<IdentityUser> userMgr,
            SignInManager<IdentityUser> signInMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
        }

        /// <summary>
        /// Retrieves login view for employees.
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ViewResult EmployeeLogin()
        {
            return View();
        }

        /// <summary>
        /// Attempt authentication for employee.
        /// </summary>
        /// <param name="accountLogin">Authentication data</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(AccountLogin accountLogin)
        {
            if (ModelState.IsValid)
            {
                var user = await _userMgr.FindByEmailAsync(accountLogin.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Authenticatie gefaald!");
                    return View(accountLogin);
                }

                // Used role based authentication instead of claim based auth (I know, claim auth is better...)
                var roles = await _userMgr.GetRolesAsync(user);

                // Check if user role contains "employee"
                if (roles != null && roles.Any() && roles.Contains("employee"))
                {
                    // Force sign out for current session
                    await _signInMgr.SignOutAsync();

                    // Redirect to main page if login is successful
                    if ((await _signInMgr.PasswordSignInAsync(user, accountLogin.Password, false, false)).Succeeded)
                    {
                        return RedirectToAction("Index", "MealPackage");
                    }
                }
            }

            // Add error message for failed authentication.
            ModelState.AddModelError("", "Authenticatie gefaald!");

            return View(accountLogin);
        }

        /// <summary>
        /// Login view for students.
        /// </summary>
        /// <returns>View</returns>
        [AllowAnonymous]
        public ViewResult StudentLogin()
        {
            return View();
        }

        /// <summary>
        /// Attempt authentication for student.
        /// </summary>
        /// <param name="accountLogin">Authentication data</param>
        /// <returns>View</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentLogin(AccountLogin accountLogin)
        {
            if (ModelState.IsValid)
            {
                var user = await _userMgr.FindByEmailAsync(accountLogin.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Authenticatie gefaald!");
                    return View(accountLogin);
                }

                var roles = await _userMgr.GetRolesAsync(user);

                // Check if user role contains "student"
                if (user != null && roles.Contains("student"))
                {
                    // Force sign out for current session
                    await _signInMgr.SignOutAsync();

                    // Redirect to main page if login is successful
                    if ((await _signInMgr.PasswordSignInAsync(user, accountLogin.Password, false, false)).Succeeded)
                    {
                        return RedirectToAction("Index", "MealPackage");
                    }
                }
            }

            // Add error message for failed authentication.
            ModelState.AddModelError("", "Authenticatie gefaald!");

            return View(accountLogin);
        }

        /// <summary>
        /// Sign out for current authenticated user.
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Logout()
        {
            await _signInMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
