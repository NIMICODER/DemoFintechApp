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
			//_contextAccessor = contextAccessor;
			_userRepo = _unitOfWork.GetRepository<ApplicationUser>();
			_accountRepo = _unitOfWork.GetRepository<Account>();
            
        }


        public async Task<ServiceResponse<CreditResponseDto>> CreditAccountAsync(string userId, CreditAccountRequestDto request)
		{
			try
			{
				

				var account = await _accountRepo.GetSingleByAsync(a => a.UserId.ToString() == userId);
				if (account == null)
					return new ServiceResponse<CreditResponseDto>
					{
						Message = "Account not found",
						StatusCode = HttpStatusCode.NotFound
					};

				account.Balance += request.Amount;
				await _accountRepo.UpdateAsync(account);
				await _unitOfWork.SaveChangesAsync();

				var creditResponseDto = new CreditResponseDto
				{
					AccountId = account.Id,
					Amount = request.Amount,
					Balance = account.Balance
				};

				return new ServiceResponse<CreditResponseDto>
				{
					Message = "Payment Successful",
					StatusCode = HttpStatusCode.OK,
					Data = creditResponseDto,
				};

			}
			catch (Exception)
			{
				return new ServiceResponse<CreditResponseDto>
				{
					Message = "An error occurred",
					StatusCode = HttpStatusCode.InternalServerError
				};
				throw;
			}
		}

		public async Task<ServiceResponse<DebitResponseDto>> DebitAccountAsync(string userId, DebitAccountRequestDto request)
		{
			try
			{
				//var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);	

				var account = await _accountRepo.GetSingleByAsync(a => a.UserId.ToString() == userId);

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
			catch (Exception ex)
			{
				return new ServiceResponse<DebitResponseDto>	
				{
					Message = "An error occrred",
					StatusCode = HttpStatusCode.InternalServerError
					
				};
			
			}
		}

		public async Task<ServiceResponse<AccountDto>> GetAccountBalanceAsync(GetAccountBalanceRequestDto request)
		{
			//string userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			Account account = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.AccountNumber);
			if (account != null)
			{
				var response = new AccountDto
				{
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

		public async Task<ServiceResponse<TransferResponseDto>> TransferFundsAsync(TransferFundsRequestDto request)
		{
			var sourceAccount = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.SourceAccountNumber);
			var destinationAccount = await _accountRepo.GetSingleByAsync(a => a.AccountNumber == request.DestinationAccountNumber);

			
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
				SenderName = sourceAccount.User.FirstName,
				ReceiverName = destinationAccount.User.FirstName,
				Amount = request.Amount,
			};
			return new ServiceResponse<TransferResponseDto>
			{
				StatusCode = HttpStatusCode.OK,
				Data = response
			};
		}
	}


}
