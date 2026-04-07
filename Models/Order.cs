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
		public string DeliveryAddress { get; set; } = string.Empty;
		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
		public decimal TotalPrice { get; set; }
		public string? Notes { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

		public Review? Review { get; set; }
	}
}
