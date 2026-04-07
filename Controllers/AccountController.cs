using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Macrofy.Models;
using Macrofy.ViewModels;
using Macrofy.Data;

namespace Macrofy.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _db;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Register(string? role = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        var vm = new RegisterViewModel();
        if (role == "chef") vm.Role = UserRole.Chef;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new ApplicationUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            FullName = vm.FullName,
            Role = vm.Role,
            EmailConfirmed = true // skip email confirmation for now
        };

        var result = await _userManager.CreateAsync(user, vm.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, vm.Role.ToString());

        // Create chef profile if registering as chef
        if (vm.Role == UserRole.Chef)
        {
            _db.ChefProfiles.Add(new ChefProfile { UserId = user.Id });
            await _db.SaveChangesAsync();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        TempData["Success"] = $"Добре дошъл, {user.FullName}! Профилът ти е създаден.";

        return vm.Role switch
        {
            UserRole.Chef => RedirectToAction("Index", "Chef"),
            _ => RedirectToAction("Index", "Dashboard")
        };
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _signInManager.PasswordSignInAsync(vm.Email, vm.Password, vm.RememberMe, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Невалиден имейл или парола.");
            return View(vm);
        }

        var user = await _userManager.FindByEmailAsync(vm.Email);
        if (user == null) return View(vm);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return user.Role switch
        {
            UserRole.Admin => RedirectToAction("Index", "Admin"),
            UserRole.Chef => RedirectToAction("Index", "Chef"),
            _ => RedirectToAction("Index", "Dashboard")
        };
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
