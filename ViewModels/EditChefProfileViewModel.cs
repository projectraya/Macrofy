using Macrofy.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Macrofy.ViewModels
{
	public class EditChefProfileViewModel
	{
		[Required, MaxLength(1000)]
		[Display(Name = "Биография")]
		public string Bio { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Специалности")]
		public string Specialties { get; set; } = string.Empty;

		[Range(0, 50)]
		[Display(Name = "Години опит")]
		public int ExperienceYears { get; set; }

		[Display(Name = "Сертификати")]
		public string? Certifications { get; set; }

		[Display(Name = "Местоположение")]
		public string? Location { get; set; }

		[Range(0, 1000)]
		[Display(Name = "Цена на час (лв)")]
		public decimal HourlyRate { get; set; }

		[Display(Name = "Специализация")]
		public ChefSpecialty Specialty { get; set; }

		[Display(Name = "Наличен за нови клиенти")]
		public bool IsAvailable { get; set; } = true;
	}
}
