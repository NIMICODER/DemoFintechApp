namespace DemoFintechApp.Shared.Dtos.Responses
{
	public record TransferResponseDto
	{
		public string SenderName { get; init; }
		public string ReceiverName { get; init; }
		public decimal Amount { get; init; }
	}


}
