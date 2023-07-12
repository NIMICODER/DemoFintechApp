using DemoFintechApp.Models.Identity;
using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;

namespace DemoFintechApp.Services.Interfaces
{

    public interface IAccountService
    {
		Task<ServiceResponse<AccountDto>> CreateAccountAsync(CreateAccountDto accountDto);
		Task<ServiceResponse<SignedInDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
		Task<SignedInDto> CreateAccessTokenAsync(ApplicationUser? user);
		Task<ServiceResponse<SignedInDto>> RefreshAccessTokenAsync(TokenDto request);


	}
}
