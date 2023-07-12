using System.ComponentModel.DataAnnotations;

namespace DemoFintechApp.Shared.Dtos.Requests
{
	public record TransferFundsRequestDto
	{
		[Required(ErrorMessage = "Source account number is required.")]
		public string SourceAccountNumber { get; init; }

		[Required(ErrorMessage = "Destination account number is required.")]
		public string DestinationAccountNumber { get; init; }

		[Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number.")]
		public long Amount { get; init; }
	}

}

