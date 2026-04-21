using Microsoft.AspNetCore.Identity;

namespace Macrofy.Models;

public class ApplicationUser : IdentityUser
{
	public string FullName { get; set; } = string.Empty;
	public UserRole Role { get; set; } = UserRole.Client;
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public string? Phone { get; set; }

	// Macro profile
	public int? Age { get; set; }
	public decimal? WeightKg { get; set; }
	public decimal? HeightCm { get; set; }
	public Gender? Gender { get; set; }
	public ActivityLevel? ActivityLevel { get; set; }
	public FitnessGoal? Goal { get; set; }
	public DietaryPreference? DietaryPreference { get; set; }
	public int? DailyCalories { get; set; }
	public int? DailyProtein { get; set; }
	public int? DailyCarbs { get; set; }
	public int? DailyFat { get; set; }

	// Allergies — stored as comma-separated string of enum values
	public string? AllergyList { get; set; }
	public bool NoAllergies { get; set; } = false;
	public bool HasAllergies { get; set; }

	// Food preferences — comma-separated
	public string? FavProteins { get; set; }
	public string? FavCarbs { get; set; }
	public string? FavFats { get; set; }
	public string? FavDesserts { get; set; }
	public string? CheatMeal { get; set; }
	public string? DietaryNotes { get; set; }

	// Navigation
	public ChefProfile? ChefProfile { get; set; }
	public ICollection<Order> Orders { get; set; } = new List<Order>();
	public ICollection<Review> Reviews { get; set; } = new List<Review>();
	public ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
}

public enum UserRole { Client, Chef, Admin }
public enum Gender { Male, Female, Other }
public enum ActivityLevel { Sedentary, Light, Moderate, Active, VeryActive }
public enum FitnessGoal { LoseWeight, Maintain, GainMuscle }
public enum DietaryPreference { Standard, Vegetarian, Vegan, Keto, GlutenFree, DairyFree }