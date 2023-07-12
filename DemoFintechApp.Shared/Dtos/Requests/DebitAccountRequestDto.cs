using System.ComponentModel.DataAnnotations;

namespace DemoFintechApp.Shared.Dtos.Requests
{
	public record DebitAccountRequestDto
	{
		[Required(ErrorMessage = "Account number is required.")]
		public string AccountNumber { get; init; }

		[Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
		public long Amount { get; init; }
	}

}

