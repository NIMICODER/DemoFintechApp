using DemoFintechApp.Data.Interfaces;
using DemoFintechApp.Models.Entities;
using DemoFintechApp.Models.Identity;
using DemoFintechApp.Services.Interfaces;
using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace DemoFintechApp.Services.Implementations
{
	public class TransactionService : ITransactionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRepository<ApplicationUser> _userRepo;
		private readonly IRepository<Account> _accountRepo;

        public TransactionService(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
			_userRepo = _unitOfWork.GetRepository<ApplicationUser>();
			_accountRepo = _unitOfWork.GetRepository<Account>();
        }

		public async Task<ServiceResponse<DebitResponseDto>> DebitAccountAsync(string userId, DebitAccountRequestDto request)
		{

				var account = await _accountRepo.GetSingleByAsync(a => a.UserId == userId);

				if (account == null)
					return new ServiceResponse<DebitResponseDto>
					{
						Message = "Account not found",
						StatusCode = HttpStatusCode.NotFound
					};
				if (account.Balance < request.Amount)
					return new ServiceResponse<DebitResponseDto>
					{
						Message = "Insufficient funds",
						StatusCode = HttpStatusCode.BadRequest
					};
				account.Balance -= request.Amount;

				await _accountRepo.UpdateAsync(account);

				var debitResponse = new DebitResponseDto
				{
					AccountId = account.Id,
					Amount = request.Amount,
					Balance = account.Balance,
				};

				return new ServiceResponse<DebitResponseDto>
				{
					Message = "Debit Successful",
					StatusCode = HttpStatusCode.OK,
					Data = debitResponse
				};
			
		}

		public async Task<ServiceResponse<AccountDto>> GetAccountBalanceAsync(GetAccountBalanceRequestDto request)
		{
			//string userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			Account account = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.AccountNumber);
			if (account != null)
			{
				var response = new AccountDto
				{
					AccountId = account.UserId,
					AccountNumber = account.AccountNumber,
					Balance = account.Balance,
				};
				
				return new ServiceResponse<AccountDto>
				{
					Message = "",
					StatusCode = HttpStatusCode.OK,
					Data = response
				};
			}
			return new ServiceResponse<AccountDto>
			{
				Message = "Account not found",
				StatusCode = HttpStatusCode.BadRequest,

			};

		}

		public async Task<ServiceResponse<CreditAccountResponseDto>> CreditAccountAsync(string userId, CreditAccountRequestDto request)
		{
			var account = await _accountRepo.GetSingleByAsync(a => a.UserId == userId);
			if (account == null)
			{
				return new ServiceResponse<CreditAccountResponseDto>
				{
					StatusCode = HttpStatusCode.NotFound,
					Message = "Account not found"
				};
			}

			account.Balance += request.Amount;


			await _accountRepo.UpdateAsync(account);
			await _unitOfWork.SaveChangesAsync();

			var response = new CreditAccountResponseDto
			{
				AccountId = account.Id,
				Balance = account.Balance,
				AmountCredited = account.Balance,
			};
			return new ServiceResponse<CreditAccountResponseDto>
			{
				StatusCode = HttpStatusCode.OK,
				Data = response
			};
		}

		public async Task<ServiceResponse<TransferResponseDto>> TransferFundsAsync(TransferFundsRequestDto request)
		{
			var sourceAccount = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.SenderAccountNumber);
			var destinationAccount = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.RecieverAccountNumber);

			
			if (sourceAccount == null || destinationAccount == null)
			{
				return new ServiceResponse<TransferResponseDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Invalid source or destination account number."
				};
			}

			if (sourceAccount.Balance < request.Amount)
			{
				return new ServiceResponse<TransferResponseDto>
				{
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Insufficient funds in the source account."
				};
			}

			sourceAccount.Balance -= request.Amount;
			destinationAccount.Balance += request.Amount;

			await _accountRepo.UpdateAsync(sourceAccount);
			await _accountRepo.UpdateAsync(destinationAccount);
			await _unitOfWork.SaveChangesAsync();

			var response = new TransferResponseDto
			{
				Amount = request.Amount,
			};
			return new ServiceResponse<TransferResponseDto>
			{
				Message = "Transfer Successful",
				StatusCode = HttpStatusCode.OK,
				Data = response
			};
		}
	}


}
