using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.ViewModels;
using Macrofy.Models.Enum;

namespace Macrofy.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(AppDbContext db, UserManager<ApplicationUser> um)
    {
        _db = db; _userManager = um;
    }

    public async Task<IActionResult> Index()
    {
        var totalUsers = await _db.Users.CountAsync();
        var totalClients = await _db.Users.CountAsync(u => u.Role == UserRole.Client);
        var totalChefs = await _db.Users.CountAsync(u => u.Role == UserRole.Chef);
        var totalOrders = await _db.Orders.CountAsync();
        var pendingOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        var pendingChefs = await _db.ChefProfiles.CountAsync(c => !c.IsVerified);
        var pendingPartners = await _db.PartnerApplications.CountAsync(p => p.Status == PartnerStatus.Pending);
        var totalRevenue = await _db.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

        var recentOrders = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ClientName = o.Client.FullName,
                ChefName = o.ChefProfile.User.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt
            }).ToListAsync();

        return View(new AdminDashboardViewModel
        {
            TotalUsers = totalUsers,
            TotalClients = totalClients,
            TotalChefs = totalChefs,
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            PendingChefApplications = pendingChefs,
            PendingPartnerApplications = pendingPartners,
            TotalRevenue = totalRevenue,
            RecentOrders = recentOrders
        });
    }

    public async Task<IActionResult> Chefs()
    {
        var chefs = await _db.ChefProfiles
            .Include(c => c.User)
            .OrderBy(c => c.IsVerified)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync();
        return View(chefs);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyChef(int id, bool verified)
    {
        var chef = await _db.ChefProfiles.FindAsync(id);
        if (chef == null) return NotFound();
        chef.IsVerified = verified;
        await _db.SaveChangesAsync();
        TempData["Success"] = verified ? "Готвачът е верифициран." : "Верификацията е премахната.";
        return RedirectToAction("Chefs");
    }

    public async Task<IActionResult> Users()
    {
        var users = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> Partners()
    {
        var apps = await _db.PartnerApplications
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return View(apps);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePartnerStatus(int id, PartnerStatus status)
    {
        var app = await _db.PartnerApplications.FindAsync(id);
        if (app == null) return NotFound();
        app.Status = status;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Статусът е обновен.";
        return RedirectToAction("Partners");
    }

    public async Task<IActionResult> Orders()
    {
        var orders = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                ClientName = o.Client.FullName,
                ChefName = o.ChefProfile.User.FullName,
                Status = o.Status,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt
            }).ToListAsync();
        return View(orders);
    }
}
