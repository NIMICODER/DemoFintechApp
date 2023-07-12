using Azure.Core;
using DemoFintechApp.API.Controllers.Shared;
using DemoFintechApp.Models.Utility;
using DemoFintechApp.Services.Interfaces;
using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoFintechApp.API.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : BaseController
	{
		private readonly IAccountService _accountService;
		public AccountController(IAccountService accountService)
		{
			_accountService = accountService;
		}


		[HttpPost("create-account")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<AccountDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto request)
		{
			ServiceResponse<AccountDto> response = await _accountService.CreateAccountAsync(request);
			return ComputeResponse(response);
		}

		[HttpPost("login-account")]
		[ProducesResponseType(200, Type = typeof(ApiResponse<AccountDto>))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
		{
			ServiceResponse<SignedInDto> response = await _accountService.LoginAsync(loginDto, cancellationToken);
			return ComputeResponse(response);
		}

		[HttpPost("refresh")]
		[ProducesResponseType(200, Type = typeof(ApiResponse))]
		[ProducesResponseType(404, Type = typeof(ApiResponse))]
		[ProducesResponseType(400, Type = typeof(ApiResponse))]
		public async Task<IActionResult> Refresh([FromBody] TokenDto request)
		{
			var tokenResponse = await _accountService.RefreshAccessTokenAsync(request);
			return ComputeResponse(tokenResponse);

		}
	}
}
