using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.ViewModels;
using Macrofy.Models.Enum;

namespace Macrofy.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(AppDbContext db, UserManager<ApplicationUser> um)
    {
        _db = db; _userManager = um;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int chefId)
    {
        var chef = await _db.ChefProfiles
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == chefId);

        if (chef == null) return NotFound();

        var vm = new CreateOrderViewModel
        {
            ChefProfileId = chefId,
            ChefName = chef.User.FullName,
            HourlyRate = chef.HourlyRate,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(9))
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var chef = await _db.ChefProfiles
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == vm.ChefProfileId);
        if (chef == null) return NotFound();

        // Calculate price (days * daily rate approx)
        var days = vm.EndDate.DayNumber - vm.StartDate.DayNumber;
        var totalPrice = days * chef.HourlyRate * 2; // 2 hours cooking per day estimate

        var order = new Order
        {
            ClientId = user.Id,
            ChefProfileId = vm.ChefProfileId,
            DeliveryAddress = vm.DeliveryAddress,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            TotalPrice = totalPrice,
            Notes = vm.Notes,
            Status = OrderStatus.Pending
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Поръчката е изпратена до {chef.User.FullName}! Ще получиш потвърждение скоро.";
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.ClientId == user.Id);
        if (order == null) return NotFound();

        if (order.Status != OrderStatus.Pending)
        {
            TempData["Error"] = "Само чакащи поръчки могат да бъдат отменени.";
            return RedirectToAction("Orders", "Dashboard");
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Поръчката е отменена.";
        return RedirectToAction("Orders", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var order = await _db.Orders
            .Include(o => o.Client)
            .Include(o => o.ChefProfile).ThenInclude(c => c.User)
            .Include(o => o.Review)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        // Only client or chef of this order can view it
        bool isClient = order.ClientId == user.Id;
        bool isChef = order.ChefProfile.UserId == user.Id;
        if (!isClient && !isChef && user.Role != UserRole.Admin)
            return Forbid();

        ViewBag.IsClient = isClient;
        return View(order);
    }
}
