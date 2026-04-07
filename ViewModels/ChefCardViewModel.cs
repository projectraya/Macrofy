using Macrofy.Models.Enum;

namespace Macrofy.ViewModels
{
	public class ChefCardViewModel
	{
		public int Id { get; set; }
		public string FullName { get; set; } = string.Empty;
		public string Bio { get; set; } = string.Empty;
		public string Specialties { get; set; } = string.Empty;
		public int ExperienceYears { get; set; }
		public decimal Rating { get; set; }
		public int TotalReviews { get; set; }
		public bool IsAvailable { get; set; }
		public bool IsVerified { get; set; }
		public string? Location { get; set; }
		public decimal HourlyRate { get; set; }
		public ChefSpecialty Specialty { get; set; }
	}
}
