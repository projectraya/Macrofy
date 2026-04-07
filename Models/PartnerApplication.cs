using Macrofy.Models.Enum;

namespace Macrofy.Models
{
	public class PartnerApplication
	{
		public int Id { get; set; }
		public string OrganizationName { get; set; } = string.Empty;
		public string ContactName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string? Phone { get; set; }
		public OrganizationType OrganizationType { get; set; }
		public string? Message { get; set; }
		public PartnerStatus Status { get; set; } = PartnerStatus.Pending;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
