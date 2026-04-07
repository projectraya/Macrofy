using System.ComponentModel.DataAnnotations;

namespace Macrofy.ViewModels
{
	public class CreateOrderViewModel
	{
		[Required]
		public int ChefProfileId { get; set; }

		[Required(ErrorMessage = "Въведи адрес за доставка")]
		[Display(Name = "Адрес за доставка")]
		public string DeliveryAddress { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Начална дата")]
		public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

		[Required]
		[Display(Name = "Крайна дата")]
		public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(9));

		[Display(Name = "Бележки")]
		public string? Notes { get; set; }

		// Chef info for display
		public string ChefName { get; set; } = string.Empty;
		public decimal HourlyRate { get; set; }
	}
}
