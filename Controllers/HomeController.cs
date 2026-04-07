using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.ViewModels;

namespace Macrofy.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        var chefs = await _db.ChefProfiles
            .Where(c => c.IsVerified && c.IsAvailable)
            .Include(c => c.User)
            .Take(3)
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

        ViewBag.FeaturedChefs = chefs;
        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
