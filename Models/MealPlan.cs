namespace Macrofy.Models
{
	public class MealPlan
	{
		public int Id { get; set; }
		public int ChefProfileId { get; set; }
		public ChefProfile ChefProfile { get; set; } = null!;

		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DietaryPreference DietaryType { get; set; }

		public int CaloriesPerDay { get; set; }
		public int ProteinPerDay { get; set; }
		public int CarbsPerDay { get; set; }
		public int FatPerDay { get; set; }

		public decimal PricePerWeek { get; set; }
		public bool IsActive { get; set; } = true;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<Order> Orders { get; set; } = new List<Order>();
	}
}
