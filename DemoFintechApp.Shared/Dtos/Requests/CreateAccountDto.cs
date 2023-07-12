using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Shared.Dtos.Requests
{
	public record CreateAccountDto
	{
		[Required(ErrorMessage = "First name is required")]
		public string? FirstName { get; init; }

		[Required(ErrorMessage = "Last name is required")]
		public string? LastName { get; init; }

		[Required(ErrorMessage = "Password is required")]
		public string? Password { get; init; }

		[Compare("Password", ErrorMessage = "Passwords do not match")]
		public string ConfirmPassword { get; init; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string? Email { get; init; }

		[Required]
		public string? Address { get; init; }

		
		//[Required, DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number")]
		//public string? PhoneNumber { get; init; }
	}
}

