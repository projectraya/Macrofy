using Macrofy.Models.Enum;

namespace Macrofy.ViewModels
{
	public class OrderViewModel
	{
		public int Id { get; set; }
		public string ClientName { get; set; } = string.Empty;
		public string ChefName { get; set; } = string.Empty;
		public OrderStatus Status { get; set; }
		public DateOnly StartDate { get; set; }
		public DateOnly EndDate { get; set; }
		public decimal TotalPrice { get; set; }
		public string? Notes { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
