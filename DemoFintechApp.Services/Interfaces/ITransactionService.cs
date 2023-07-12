using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;

namespace DemoFintechApp.Services.Interfaces
{
	public interface ITransactionService
    {
		Task<ServiceResponse<DebitResponseDto>> DebitAccountAsync(string userId, DebitAccountRequestDto request);
		Task<ServiceResponse<CreditResponseDto>> CreditAccountAsync(string userId, CreditAccountRequestDto request);
		Task<ServiceResponse<AccountDto>> GetAccountBalanceAsync(GetAccountBalanceRequestDto request);
		Task<ServiceResponse<TransferResponseDto>> TransferFundsAsync(TransferFundsRequestDto request);

	}
}
