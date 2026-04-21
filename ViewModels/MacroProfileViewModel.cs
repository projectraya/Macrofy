using Macrofy.Models;
using System.ComponentModel.DataAnnotations;

namespace Macrofy.ViewModels
{
	public class MacroProfileViewModel
	{
		[Required, Range(15, 80)]
		[Display(Name = "Възраст")]
		public int Age { get; set; }

		[Required, Range(30, 300)]
		[Display(Name = "Тегло (кг)")]
		public decimal WeightKg { get; set; }

		[Required, Range(100, 250)]
		[Display(Name = "Ръст (см)")]
		public decimal HeightCm { get; set; }

		[Required]
		[Display(Name = "Пол")]
		public Gender Gender { get; set; }

		[Required]
		[Display(Name = "Ниво на активност")]
		public ActivityLevel ActivityLevel { get; set; }

		[Required]
		[Display(Name = "Цел")]
		public FitnessGoal Goal { get; set; }

		[Display(Name = "Диетични предпочитания")]
		public DietaryPreference DietaryPreference { get; set; }

		[Display(Name = "Има ли алергии")]
		public bool HasAllergies { get; set; }

		[Display(Name = "Алергии")]
		public string? AllergyList { get; set; }

		public string? FavProteins { get; set; }
		public string? FavCarbs { get; set; }
		public string? FavFats { get; set; }
		public string? FavDesserts { get; set; }

		public string? DietaryNotes { get; set; }
	}
}
