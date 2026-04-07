namespace Macrofy.Models
{
	public class Review
	{
		public int Id { get; set; }
		public string ClientId { get; set; } = string.Empty;
		public ApplicationUser Client { get; set; } = null!;

		public int ChefProfileId { get; set; }
		public ChefProfile ChefProfile { get; set; } = null!;

		public int OrderId { get; set; }
		public Order Order { get; set; } = null!;

		public int Rating { get; set; } // 1-5
		public string? Comment { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
