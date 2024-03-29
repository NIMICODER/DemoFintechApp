﻿namespace DemoFintechApp.Shared.Dtos.Responses
{
	public class CreditResponseDto
	{
		public Guid AccountId { get; set; }
		public decimal Amount { get; set; }
		public decimal Balance { get; set; }
	}

	public class CreditAccountResponseDto
	{
		public Guid AccountId { get; set; }
		public decimal Balance { get; set; }
		public decimal AmountCredited { get; set; }
	}


}
