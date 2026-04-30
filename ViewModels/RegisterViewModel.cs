using Macrofy.Models;
using System.ComponentModel.DataAnnotations;

namespace Macrofy.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Въведи пълното си име")]
		[Display(Name = "Пълно име")]
		public string FullName { get; set; } = string.Empty;

		[Required, EmailAddress(ErrorMessage = "Невалиден имейл")]
		[Display(Name = "Имейл")]
		public string Email { get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Парола")]
		public string Password { get; set; } = string.Empty;

		[Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Паролите не съвпадат")]
		[Display(Name = "Потвърди паролата")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[Display(Name = "Регистрирай се като")]
		public UserRole Role { get; set; } = UserRole.Client;
	}
}
