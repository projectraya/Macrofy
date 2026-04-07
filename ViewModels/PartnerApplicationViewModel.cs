using Macrofy.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Macrofy.ViewModels
{
	public class PartnerApplicationViewModel
	{
		[Required(ErrorMessage = "Въведи името на организацията")]
		[Display(Name = "Организация")]
		public string OrganizationName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Въведи контактно лице")]
		[Display(Name = "Контактно лице")]
		public string ContactName { get; set; } = string.Empty;

		[Required, EmailAddress]
		[Display(Name = "Имейл")]
		public string Email { get; set; } = string.Empty;

		[Display(Name = "Телефон")]
		public string? Phone { get; set; }

		[Display(Name = "Тип организация")]
		public OrganizationType OrganizationType { get; set; }

		[Display(Name = "Съобщение")]
		public string? Message { get; set; }
	}
}
