using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;

namespace DemoFintechApp.Services.Interfaces
{
	public interface ITransactionService
    {
		Task<ServiceResponse<DebitResponseDto>> DebitAccountAsync(string userId, DebitAccountRequestDto request);
		Task<ServiceResponse<AccountDto>> GetAccountBalanceAsync(GetAccountBalanceRequestDto request);
		Task<ServiceResponse<TransferResponseDto>> TransferFundsAsync(TransferFundsRequestDto request);

		Task<ServiceResponse<CreditAccountResponseDto>> CreditAccountAsync(string userId, CreditAccountRequestDto request);
	}
}
