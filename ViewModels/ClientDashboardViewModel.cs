using Macrofy.Models;

namespace Macrofy.ViewModels
{
	public class ClientDashboardViewModel
	{
		public string FullName { get; set; } = string.Empty;
		public bool HasMacroProfile { get; set; }
		public int? Calories { get; set; }
		public int? Protein { get; set; }
		public int? Carbs { get; set; }
		public int? Fat { get; set; }
		public DietaryPreference? DietaryPreference { get; set; }
		public List<OrderViewModel> RecentOrders { get; set; } = new();
		public List<ChefCardViewModel> AvailableChefs { get; set; } = new();
		public ChefCardViewModel? AssignedChef { get; set; }
	}
}
