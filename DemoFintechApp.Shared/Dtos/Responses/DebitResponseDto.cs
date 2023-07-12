using System.Net;

namespace DemoFintechApp.Shared.Dtos.Responses
{
	public class DebitResponseDto
	{
		public Guid AccountId { get; set; }
		public decimal Amount { get; set; }
		public decimal Balance { get; set; }

		
	}


}
