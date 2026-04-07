using Microsoft.AspNetCore.Mvc;
using Macrofy.Data;
using Macrofy.Models;
using Macrofy.ViewModels;

namespace Macrofy.Controllers;

public class PartnerController : Controller
{
    private readonly AppDbContext _db;
    public PartnerController(AppDbContext db) { _db = db; }

    [HttpGet]
    public IActionResult Apply() => View(new PartnerApplicationViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(PartnerApplicationViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        _db.PartnerApplications.Add(new PartnerApplication
        {
            OrganizationName = vm.OrganizationName,
            ContactName = vm.ContactName,
            Email = vm.Email,
            Phone = vm.Phone,
            OrganizationType = vm.OrganizationType,
            Message = vm.Message
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Получихме вашето запитване! Ще се свържем с вас скоро.";
        return RedirectToAction("Apply");
    }
}
