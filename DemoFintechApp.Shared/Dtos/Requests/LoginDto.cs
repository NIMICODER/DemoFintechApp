using System.ComponentModel.DataAnnotations;

namespace DemoFintechApp.Shared.Dtos.Requests
{
	public record LoginDto
	{
		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string? Email { get; init; }

		[Required(ErrorMessage = "Password is required")]
		public string? Password { get; init; }

	}

}

