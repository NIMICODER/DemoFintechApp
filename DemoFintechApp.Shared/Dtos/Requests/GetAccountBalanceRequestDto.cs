using System.ComponentModel.DataAnnotations;

namespace DemoFintechApp.Shared.Dtos.Requests
{
	public record GetAccountBalanceRequestDto
	{
		[Required(ErrorMessage = "Account number is required.")]
		public string AccountNumber { get; init; }
	}

}

