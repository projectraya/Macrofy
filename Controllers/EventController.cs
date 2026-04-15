using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.Models.Enum;

namespace Macrofy.Controllers;

public class EventController : Controller
{
	private readonly AppDbContext _db;
	private readonly UserManager<ApplicationUser> _userManager;

	public EventController(AppDbContext db, UserManager<ApplicationUser> um)
	{ _db = db; _userManager = um; }

	public async Task<IActionResult> Index()
	{
		var events = await _db.Events
			.Where(e => e.IsActive)
			.Include(e => e.Registrations)
			.OrderBy(e => e.EventDate)
			.ToListAsync();
		return View(events);
	}

	[HttpGet]
	[Authorize(Roles = "Client")]
	public async Task<IActionResult> Register(int id)
	{
		var ev = await _db.Events
			.Include(e => e.Registrations)
			.FirstOrDefaultAsync(e => e.Id == id);
		if (ev == null) return NotFound();

		var user = await _userManager.GetUserAsync(User);
		if (user == null) return RedirectToAction("Login", "Account");

		// Already registered?
		if (ev.Registrations.Any(r => r.UserId == user.Id))
		{
			TempData["Error"] = "Вече си записан за това събитие.";
			return RedirectToAction("Index");
		}

		// Full?
		var confirmed = ev.Registrations.Count(r => r.Status != RegistrationStatus.Rejected);
		if (confirmed >= ev.MaxSpots)
		{
			TempData["Error"] = "За съжаление местата са изчерпани.";
			return RedirectToAction("Index");
		}

		// No macro profile?
		if (!user.DailyCalories.HasValue)
		{
			TempData["Error"] = "Трябва да попълниш макро профила си преди да се запишеш за събитие.";
			return RedirectToAction("MacroProfile", "Dashboard", new { returnUrl = Url.Action("Register", "Event", new { id }) });
		}

		ViewBag.Event = ev;
		ViewBag.User = user;
		return View();
	}

	[HttpPost]
	[Authorize(Roles = "Client")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(int id, string contactEmail, string? contactPhone)
	{
		var ev = await _db.Events.Include(e => e.Registrations).FirstOrDefaultAsync(e => e.Id == id);
		if (ev == null) return NotFound();

		var user = await _userManager.GetUserAsync(User);
		if (user == null) return RedirectToAction("Login", "Account");

		if (ev.Registrations.Any(r => r.UserId == user.Id))
		{ TempData["Error"] = "Вече си записан."; return RedirectToAction("Index"); }

		var confirmed = ev.Registrations.Count(r => r.Status != RegistrationStatus.Rejected);
		if (confirmed >= ev.MaxSpots)
		{ TempData["Error"] = "Местата са изчерпани."; return RedirectToAction("Index"); }

		_db.EventRegistrations.Add(new EventRegistration
		{
			EventId = id,
			UserId = user.Id,
			ContactEmail = contactEmail,
			ContactPhone = contactPhone,
			Status = RegistrationStatus.Pending
		});
		await _db.SaveChangesAsync();

		TempData["Success"] = "Записан си успешно! Ще получиш потвърждение скоро.";
		return RedirectToAction("Index");
	}
}