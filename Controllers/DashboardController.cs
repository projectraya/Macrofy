using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.Services;
using Macrofy.ViewModels;
using Macrofy.Models.Enum;

namespace Macrofy.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly MacroCalculatorService _macroCalc;

    public DashboardController(AppDbContext db, UserManager<ApplicationUser> um, MacroCalculatorService mc)
    {
        _db = db; _userManager = um; _macroCalc = mc;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // Route by role
        if (user.Role == UserRole.Chef) return RedirectToAction("Index", "Chef");
        if (user.Role == UserRole.Admin) return RedirectToAction("Index", "Admin");

        var recentOrders = await _db.Orders
            .Where(o => o.ClientId == user.Id)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ChefName = o.ChefProfile.User.FullName,
                ClientName = user.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

        // Active chef (latest confirmed order)
        var activeOrder = await _db.Orders
            .Where(o => o.ClientId == user.Id && o.Status == OrderStatus.Confirmed)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        ChefCardViewModel? assignedChef = null;
        if (activeOrder != null)
        {
            var cp = activeOrder.ChefProfile;
            assignedChef = new ChefCardViewModel
            {
                Id = cp.Id,
                FullName = cp.User.FullName,
                Bio = cp.Bio,
                Specialties = cp.Specialties,
                Rating = cp.Rating,
                TotalReviews = cp.TotalReviews,
                IsAvailable = cp.IsAvailable,
                IsVerified = cp.IsVerified,
                Location = cp.Location,
                HourlyRate = cp.HourlyRate,
                Specialty = cp.Specialty
            };
        }

        var availableChefs = await _db.ChefProfiles
            .Where(c => c.IsVerified && c.IsAvailable)
            .Include(c => c.User)
            .Take(6)
            .Select(c => new ChefCardViewModel
            {
                Id = c.Id,
                FullName = c.User.FullName,
                Bio = c.Bio,
                Specialties = c.Specialties,
                ExperienceYears = c.ExperienceYears,
                Rating = c.Rating,
                TotalReviews = c.TotalReviews,
                IsAvailable = c.IsAvailable,
                IsVerified = c.IsVerified,
                Location = c.Location,
                HourlyRate = c.HourlyRate,
                Specialty = c.Specialty
            }).ToListAsync();

        var vm = new ClientDashboardViewModel
        {
            FullName = user.FullName,
            HasMacroProfile = user.DailyCalories.HasValue,
            Calories = user.DailyCalories,
            Protein = user.DailyProtein,
            Carbs = user.DailyCarbs,
            Fat = user.DailyFat,
            DietaryPreference = user.DietaryPreference,
            RecentOrders = recentOrders,
            AvailableChefs = availableChefs,
            AssignedChef = assignedChef
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> MacroProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var vm = new MacroProfileViewModel
        {
            Age = user.Age ?? 25,
            WeightKg = user.WeightKg ?? 70,
            HeightCm = user.HeightCm ?? 175,
            Gender = user.Gender ?? Models.Gender.Male,
            ActivityLevel = user.ActivityLevel ?? Models.ActivityLevel.Moderate,
            Goal = user.Goal ?? Models.FitnessGoal.Maintain,
            DietaryPreference = user.DietaryPreference ?? Models.DietaryPreference.Standard,
            Allergies = user.Allergies
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MacroProfile(MacroProfileViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var result = _macroCalc.Calculate(vm.Age, vm.WeightKg, vm.HeightCm, vm.Gender, vm.ActivityLevel, vm.Goal);

        user.Age = vm.Age;
        user.WeightKg = vm.WeightKg;
        user.HeightCm = vm.HeightCm;
        user.Gender = vm.Gender;
        user.ActivityLevel = vm.ActivityLevel;
        user.Goal = vm.Goal;
        user.DietaryPreference = vm.DietaryPreference;
        user.Allergies = vm.Allergies;
        user.DailyCalories = result.Calories;
        user.DailyProtein = result.Protein;
        user.DailyCarbs = result.Carbs;
        user.DailyFat = result.Fat;

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Макро профилът е обновен!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Chefs(string? specialty = null)
    {
        var query = _db.ChefProfiles
            .Where(c => c.IsVerified)
            .Include(c => c.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(specialty) && Enum.TryParse<ChefSpecialty>(specialty, out var sp))
            query = query.Where(c => c.Specialty == sp);

        var chefs = await query
            .OrderByDescending(c => c.Rating)
            .Select(c => new ChefCardViewModel
            {
                Id = c.Id,
                FullName = c.User.FullName,
                Bio = c.Bio,
                Specialties = c.Specialties,
                ExperienceYears = c.ExperienceYears,
                Rating = c.Rating,
                TotalReviews = c.TotalReviews,
                IsAvailable = c.IsAvailable,
                IsVerified = c.IsVerified,
                Location = c.Location,
                HourlyRate = c.HourlyRate,
                Specialty = c.Specialty
            }).ToListAsync();

        ViewBag.CurrentSpecialty = specialty;
        return View(chefs);
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var orders = await _db.Orders
            .Where(o => o.ClientId == user.Id)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ChefName = o.ChefProfile.User.FullName,
                ClientName = user.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

        return View(orders);
    }
}
