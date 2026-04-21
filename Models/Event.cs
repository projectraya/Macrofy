namespace Macrofy.Models
{
	public class Event
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public DateTime EventDate { get; set; }
		public int MaxSpots { get; set; } = 10;

		public int ConfirmedSpots { get; set; } = 0;
		public bool IsActive { get; set; } = true;
		public string? ImageUrl { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
	}
}
