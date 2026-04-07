using Microsoft.AspNetCore.Identity;
using Macrofy.Models;
using Macrofy.Models.Enum;

namespace Macrofy.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.EnsureCreatedAsync();

        // Seed roles
        foreach (var role in new[] { "Admin", "Chef", "Client" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed admin
        if (await userManager.FindByEmailAsync("admin@macrofy.bg") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@macrofy.bg",
                Email = "admin@macrofy.bg",
                FullName = "Macrofy Admin",
                Role = UserRole.Admin,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin@123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed demo chef
        if (await userManager.FindByEmailAsync("ivan@macrofy.bg") == null)
        {
            var chef = new ApplicationUser
            {
                UserName = "ivan@macrofy.bg",
                Email = "ivan@macrofy.bg",
                FullName = "Иван Петров",
                Role = UserRole.Chef,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(chef, "Chef@123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(chef, "Chef");
                db.ChefProfiles.Add(new ChefProfile
                {
                    UserId = chef.Id,
                    Bio = "Специалист по спортно хранене с 5 години опит. Работил съм с над 50 клиенти, помагайки им да постигнат своите фитнес цели чрез прецизно планирано хранене.",
                    Specialties = "Спортно хранене, Кето, Набиране на мускулна маса",
                    ExperienceYears = 5,
                    Certifications = "НСА Сертификат по хранене, Курс по спортна диетология",
                    Location = "София",
                    HourlyRate = 25m,
                    IsVerified = true,
                    IsAvailable = true,
                    Rating = 4.9m,
                    TotalReviews = 23,
                    Specialty = ChefSpecialty.SportNutrition
                });
                await db.SaveChangesAsync();
            }
        }

        // Seed demo client
        if (await userManager.FindByEmailAsync("demo@macrofy.bg") == null)
        {
            var client = new ApplicationUser
            {
                UserName = "demo@macrofy.bg",
                Email = "demo@macrofy.bg",
                FullName = "Мария Димитрова",
                Role = UserRole.Client,
                EmailConfirmed = true,
                Age = 28,
                WeightKg = 65m,
                HeightCm = 168m,
                Gender = Gender.Female,
                ActivityLevel = ActivityLevel.Moderate,
                Goal = FitnessGoal.LoseWeight,
                DietaryPreference = DietaryPreference.Standard,
                DailyCalories = 1800,
                DailyProtein = 130,
                DailyCarbs = 160,
                DailyFat = 60
            };
            var result = await userManager.CreateAsync(client, "Client@123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(client, "Client");
        }
    }
}
