using Macrofy.Models;

namespace Macrofy.ViewModels
{
	public class ChefDashboardViewModel
	{
		public string FullName { get; set; } = string.Empty;
		public ChefProfile? Profile { get; set; }
		public List<OrderViewModel> PendingOrders { get; set; } = new();
		public List<OrderViewModel> ActiveOrders { get; set; } = new();
		public int TotalClients { get; set; }
		public decimal TotalEarnings { get; set; }
		public decimal Rating { get; set; }
	}
}
