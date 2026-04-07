namespace Macrofy.ViewModels
{
	public class AdminDashboardViewModel
	{
		public int TotalUsers { get; set; }
		public int TotalClients { get; set; }
		public int TotalChefs { get; set; }
		public int TotalOrders { get; set; }
		public int PendingOrders { get; set; }
		public int PendingChefApplications { get; set; }
		public int PendingPartnerApplications { get; set; }
		public decimal TotalRevenue { get; set; }
		public List<OrderViewModel> RecentOrders { get; set; } = new();
	}
}
