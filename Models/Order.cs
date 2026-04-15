using Macrofy.Models.Enum;

namespace Macrofy.Models
{
	public class Order
	{
		public int Id { get; set; }
		public string ClientId { get; set; } = string.Empty;
		public ApplicationUser Client { get; set; } = null!;
		public int ChefProfileId { get; set; }
		public ChefProfile ChefProfile { get; set; } = null!;
		public int? MealPlanId { get; set; }
		public MealPlan? MealPlan { get; set; }

		public OrderStatus Status { get; set; } = OrderStatus.Pending;
		public MenuType MenuType { get; set; } = MenuType.Maintain;
		public string DeliveryAddress { get; set; } = string.Empty;
		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
		public decimal TotalPrice { get; set; }
		public string? Notes { get; set; }

		// Snapshot of client macros at time of order
		public int? SnapshotCalories { get; set; }
		public int? SnapshotProtein { get; set; }
		public int? SnapshotCarbs { get; set; }
		public int? SnapshotFat { get; set; }
		public string? SnapshotAllergies { get; set; }
		public string? SnapshotPreferences { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
		public Review? Review { get; set; }
	}
	public enum MenuType { LoseWeight, Maintain, GainMuscle }
}
