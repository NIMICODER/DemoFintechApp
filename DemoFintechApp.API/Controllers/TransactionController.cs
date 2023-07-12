using DemoFintechApp.API.Controllers.Shared;
using DemoFintechApp.Models.Utility;
using DemoFintechApp.Services.Interfaces;
using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoFintechApp.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionController : BaseController
	{
		private readonly ITransactionService _transactionService;
		private readonly IHttpContextAccessor _contextAccessor;

		public TransactionController(ITransactionService transactionService, IHttpContextAccessor contextAccessor)
		{
			_transactionService = transactionService;
			_contextAccessor = contextAccessor;
		}



		[HttpGet("get-balance")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<AccountDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> GetAccountBalance([FromQuery] GetAccountBalanceRequestDto request)
		{
			ServiceResponse<AccountDto> response = await _transactionService.GetAccountBalanceAsync(request);
			return ComputeResponse(response);
		}

		[HttpPost("debit-account")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<DebitResponseDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> DebitAccount([FromBody] DebitAccountRequestDto request) 
		{
			//var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			string userId = GetUserId();
			ServiceResponse<DebitResponseDto> response = await _transactionService.DebitAccountAsync(userId, request);
			return ComputeResponse(response);
		}

		[HttpPost("credit-account")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<CreditResponseDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> CreditAccount([FromBody] CreditAccountRequestDto request)
		{
			//var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			string userId = GetUserId();
			ServiceResponse<CreditResponseDto> response = await _transactionService.CreditAccountAsync(userId, request);
			return ComputeResponse(response);
		}

		[HttpPost("transfer-funds")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<TransferResponseDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> Transfer([FromBody] TransferFundsRequestDto request)
		{
			ServiceResponse<TransferResponseDto> response = await _transactionService.TransferFundsAsync(request);
			return ComputeResponse(response);
		}
	}
}
