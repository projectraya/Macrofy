using Macrofy.Models.Enum;

namespace Macrofy.Models
{
	public class EventRegistration
	{
		public int Id { get; set; }
		public int EventId { get; set; }
		public Event Event { get; set; } = null!;
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser User { get; set; } = null!;
		public string ContactEmail { get; set; } = string.Empty;
		public string? ContactPhone { get; set; }
		public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
		public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
	}
}
