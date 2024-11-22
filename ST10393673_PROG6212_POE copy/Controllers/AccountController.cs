using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST10393673_PROG6212_POE.Models;
using ST10393673_PROG6212_POE.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TableService _tableService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TableService tableService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tableService = tableService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Add job titles to the ViewData to be used in the Register view
            ViewData["JobTitles"] = Enum.GetValues(typeof(JobTitle)).Cast<JobTitle>().ToList();
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new IdentityUser for the user
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Optionally, you can assign a role based on the job title here
                    if (model.SelectedJobTitle != JobTitle.Lecturer)
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedJobTitle.ToString());
                    }

                    // Hash the password for storage in Azure Table Storage
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    var hashedPassword = passwordHasher.HashPassword(user, model.Password);

                    // Create the UserEntity and store it in Azure Table Storage
                    var userEntity = new UserEntity(model.Email)
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        JobTitle = model.SelectedJobTitle.ToString(),
                        Email = model.Email,
                        PasswordHash = hashedPassword // Store the hashed password
                    };

                    // Get or create the 'UserProfiles' table in Azure Table Storage
                    var table = await _tableService.GetOrCreateTableAsync("UserProfiles");
                    var success = await _tableService.InsertOrMergeEntityAsync(table, userEntity);

                    if (success)
                    {
                        // Sign the user in automatically after registration
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Redirect after successful registration
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "There was an error saving the user profile.");
                    }
                }
                else
                {
                    // Add all errors to ModelState and return to the registration page
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got here, something failed, so return the model with validation errors
            ViewData["JobTitles"] = Enum.GetValues(typeof(JobTitle)).Cast<JobTitle>().ToList(); // Ensure job titles are still available
            return View(model);
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the model with validation errors if not valid
            }

            // Find user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt."); // Error for invalid email
                return View(model);
            }

            // Attempt sign-in
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // Redirect to home page or another secure page upon successful login
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                // Account is locked out, notify the user
                ModelState.AddModelError(string.Empty, "Your account is locked.");
            }
            else if (result.IsNotAllowed)
            {
                // Login is not allowed for this user
                ModelState.AddModelError(string.Empty, "Login not allowed.");
            }
            else
            {
                // Invalid login attempt
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // Return the model to the view with any validation errors
            return View(model);
        }
    }
}
