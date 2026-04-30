using Macrofy.Models.Enum;

namespace Macrofy.Models;

public class ChefProfile
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Bio { get; set; } = string.Empty;
    public string Specialties { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string? Certifications { get; set; }
    public string? Location { get; set; }
    public decimal HourlyRate { get; set; }
    public string? PhotoUrl { get; set; }

    public bool IsVerified { get; set; } = false;
    public bool IsAvailable { get; set; } = true;
    public decimal Rating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;

    public ChefSpecialty Specialty { get; set; } = ChefSpecialty.General;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public bool HasEventBadge { get; set; } = false;
	public string? EventBadgeLabel { get; set; }

	// Navigation
	public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}


