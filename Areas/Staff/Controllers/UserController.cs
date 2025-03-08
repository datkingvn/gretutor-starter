using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using GreTutor.Areas.Staff.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace GreTutor.Areas.Staff.Controllers
{
    [Area("Staff")]
    // [Authorize(Roles = "Staff")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<UserController> _logger;
        private const int ITEMS_PER_PAGE = 10;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int p = 1)
        {
            var query = _userManager.Users.OrderBy(u => u.UserName);
            int totalUsers = await query.CountAsync();
            int countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);

            p = Math.Clamp(p, 1, countPages);

            var users = await query.Skip((p - 1) * ITEMS_PER_PAGE)
                                   .Take(ITEMS_PER_PAGE)
                                   .Select(u => new UserAndRole
                                   {
                                       Id = u.Id,
                                       UserName = u.UserName
                                   })
                                   .ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(", ", roles);
            }

            ViewData["CurrentPage"] = p;
            ViewData["CountPages"] = countPages;
            ViewData["TotalUsers"] = totalUsers;

            return View(users);
        }
        public async Task<IActionResult> AddRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound("No User");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"User Not Found, id = {id}.");

            var model = new AddRoleModel
            {
                User = user,
                RoleNames = (await _userManager.GetRolesAsync(user)).ToArray(),
                AllRoles = new SelectList(await _roleManager.Roles.Select(r => r.Name).ToListAsync())
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(string id, AddRoleModel model)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound("No User");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"User Not Found, id = {id}.");

            var oldRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = oldRoles.Where(r => !model.RoleNames.Contains(r));
            var rolesToAdd = model.RoleNames.Where(r => !oldRoles.Contains(r));

            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            TempData["StatusMessage"] = $"Roles updated for user: {user.UserName}";
            return RedirectToAction("Index");
        }
        // [HttpGet]
        // public async Task<IActionResult> DeletePersonalData(string id)
        // {
        //     if (string.IsNullOrEmpty(id))
        //     {
        //         return BadRequest("User ID is missing.");
        //     }

        //     var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        //     if (user == null)
        //     {
        //         return NotFound($"User with ID {id} not found.");
        //     }

        //     var model = new DeletePersonalDataModel
        //     {
        //         UserId = user.Id, // ✅ Lưu ID user đúng
        //         RequirePassword = await _userManager.HasPasswordAsync(user)
        //     };

        //     return View(model);
        // }

        // [HttpPost]
        // public async Task<IActionResult> DeletePersonalData(DeletePersonalDataModel model)
        // {
        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null)
        //     {
        //         return NotFound("User not found.");
        //     }

        //     if (model.RequirePassword)
        //     {
        //         if (!await _userManager.CheckPasswordAsync(user, model.Input.Password))
        //         {
        //             ModelState.AddModelError(string.Empty, "Incorrect password.");
        //             return View(model);
        //         }
        //     }

        //     var result = await _userManager.DeleteAsync(user);
        //     if (!result.Succeeded)
        //     {
        //         ModelState.AddModelError(string.Empty, "Failed to delete account.");
        //         return View(model);
        //     }

        //     await _signInManager.SignOutAsync();
        //     _logger.LogInformation("User {UserId} deleted their account.", user.Id);

        //     return RedirectToAction("Index", "Home");
        // }

    }
}

