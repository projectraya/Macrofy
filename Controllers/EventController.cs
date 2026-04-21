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

	[HttpPost]
	[Authorize(Roles = "Client")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Register(int id, string contactEmail, string? contactPhone)
	{
		var ev = await _db.Events
			.Include(e => e.Registrations)
			.FirstOrDefaultAsync(e => e.Id == id && e.IsActive);

		if (ev == null) return NotFound();

		var user = await _userManager.GetUserAsync(User);
		if (user == null) return RedirectToAction("Login", "Account");

		// Already applied?
		if (ev.Registrations.Any(r => r.UserId == user.Id))
		{
			TempData["Error"] = "Вече си кандидатствал за това събитие.";
			return RedirectToAction("Index");
		}

		// No macro profile?
		if (!user.DailyCalories.HasValue)
		{
			TempData["Error"] = "Трябва да попълниш макро профила си преди да кандидатстваш.";
			return RedirectToAction("MacroProfile", "Dashboard",
				new { returnUrl = Url.Action("Index", "Event") });
		}

		// Add application — status is always Pending, admin decides
		_db.EventRegistrations.Add(new EventRegistration
		{
			EventId = id,
			UserId = user.Id,
			ContactEmail = contactEmail,
			ContactPhone = contactPhone,
			Status = RegistrationStatus.Pending
		});
		await _db.SaveChangesAsync();

		TempData["Success"] = "Кандидатурата ти е получена! Ще се свържем с теб ако бъдеш избран.";
		return RedirectToAction("Index");
	}
}