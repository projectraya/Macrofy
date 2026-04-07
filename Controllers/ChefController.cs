using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.ViewModels;
using Macrofy.Models.Enum;

namespace Macrofy.Controllers;

[Authorize(Roles = "Chef")]
public class ChefController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChefController(AppDbContext db, UserManager<ApplicationUser> um)
    {
        _db = db; _userManager = um;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _db.ChefProfiles
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        var pendingOrders = await _db.Orders
            .Where(o => o.ChefProfile.UserId == user.Id && o.Status == OrderStatus.Pending)
            .Include(o => o.Client)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ClientName = o.Client.FullName,
                ChefName = user.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

        var activeOrders = await _db.Orders
            .Where(o => o.ChefProfile.UserId == user.Id &&
                   (o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Preparing))
            .Include(o => o.Client)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ClientName = o.Client.FullName,
                ChefName = user.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                Notes = o.Notes,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

        var totalEarnings = await _db.Orders
            .Where(o => o.ChefProfile.UserId == user.Id && o.Status == OrderStatus.Delivered)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        var totalClients = await _db.Orders
            .Where(o => o.ChefProfile.UserId == user.Id)
            .Select(o => o.ClientId)
            .Distinct()
            .CountAsync();

        var vm = new ChefDashboardViewModel
        {
            FullName = user.FullName,
            Profile = profile,
            PendingOrders = pendingOrders,
            ActiveOrders = activeOrders,
            TotalClients = totalClients,
            TotalEarnings = totalEarnings,
            Rating = profile?.Rating ?? 0
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _db.ChefProfiles.FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (profile == null)
        {
            profile = new ChefProfile { UserId = user.Id };
            _db.ChefProfiles.Add(profile);
            await _db.SaveChangesAsync();
        }

        var vm = new EditChefProfileViewModel
        {
            Bio = profile.Bio,
            Specialties = profile.Specialties,
            ExperienceYears = profile.ExperienceYears,
            Certifications = profile.Certifications,
            Location = profile.Location,
            HourlyRate = profile.HourlyRate,
            Specialty = profile.Specialty,
            IsAvailable = profile.IsAvailable
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditChefProfileViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _db.ChefProfiles.FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (profile == null) return NotFound();

        profile.Bio = vm.Bio;
        profile.Specialties = vm.Specialties;
        profile.ExperienceYears = vm.ExperienceYears;
        profile.Certifications = vm.Certifications;
        profile.Location = vm.Location;
        profile.HourlyRate = vm.HourlyRate;
        profile.Specialty = vm.Specialty;
        profile.IsAvailable = vm.IsAvailable;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Профилът е обновен успешно!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var order = await _db.Orders
            .Include(o => o.ChefProfile)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.ChefProfile.UserId == user.Id);

        if (order == null) return NotFound();

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Статусът на поръчката е обновен.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Clients()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var clients = await _db.Orders
            .Where(o => o.ChefProfile.UserId == user.Id)
            .Include(o => o.Client)
            .GroupBy(o => o.ClientId)
            .Select(g => new
            {
                Client = g.First().Client,
                OrderCount = g.Count(),
                LastOrder = g.Max(o => o.CreatedAt),
                TotalSpent = g.Sum(o => o.TotalPrice)
            })
            .ToListAsync();

        ViewBag.Clients = clients;
        return View();
    }
}
