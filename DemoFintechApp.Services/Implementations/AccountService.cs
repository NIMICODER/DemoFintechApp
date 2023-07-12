using DemoFintechApp.Data.Interfaces;
using DemoFintechApp.Models.Entities;
using DemoFintechApp.Models.Enums;
using DemoFintechApp.Models.Identity;
using DemoFintechApp.Services.Interfaces;
using DemoFintechApp.Services.Utility;
using DemoFintechApp.Shared.Dtos.Requests;
using DemoFintechApp.Shared.Dtos.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoFintechApp.Services.Implementations
{
	public class AccountService : IAccountService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly JwtConfig _jwtConfig;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IRepository<Account> _accountRepo;

		private ApplicationUser _user;
		private TimeSpan RefreshTokenValidity = TimeSpan.FromDays(7);


		public AccountService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, JwtConfig jwtConfig)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
			_accountRepo = _unitOfWork.GetRepository<Account>();
			_jwtConfig = jwtConfig;

		}

		public async Task<ServiceResponse<AccountDto>> CreateAccountAsync(CreateAccountDto model)
		{
			ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
			if (user != null)
			{
				return new ServiceResponse<AccountDto>
				{
					Message = "An account already exists with this email.",
					StatusCode = HttpStatusCode.BadRequest,
				};
			}

			if (!string.Equals(model.Password, model.ConfirmPassword))
			{
				return new ServiceResponse<AccountDto>
				{
					Message = "Password mismatch",
					StatusCode = HttpStatusCode.BadRequest
				};
			}

			var newuser = new ApplicationUser
			{
				Email = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				UserName = model.Email,
				Address = model.Address,
				Accounts  = new List<Account>
				 {
					 new Account
					 {
						 Id = Guid.NewGuid(),
						 AccountNumber = await GenerateUniqueAccountNumber(),
						Balance = 0
					 }
				 }
				
			};

			IdentityResult userCreationResult = await _userManager.CreateAsync(newuser, model.Password);
			if (!userCreationResult.Succeeded)
			{
				string? message = userCreationResult.Errors.Select(e => e.Description).FirstOrDefault();
				return new ServiceResponse<AccountDto>
				{
					Message = $"Account creation failed with reason: {message}",
					StatusCode = HttpStatusCode.BadRequest
				};
			}


			string userRole = UserRole.User.GetStringValue();
			bool roleExist = await _roleManager.RoleExistsAsync(userRole);
			if (!roleExist)
			{
				await _roleManager.CreateAsync(new IdentityRole(userRole));
			}
			await _userManager.AddToRoleAsync(newuser, userRole);

			
			//await _accountRepo.AddAsync(account);
			//await _unitOfWork.SaveChangesAsync();

			//var accountDto = new AccountDto
			//{
			//	AccountNumber = account.AccountNumber,
			//	Balance = account.Balance
			//};
			return new ServiceResponse<AccountDto>
			{
				Message = "Acount Created Succefully",
				Data = new AccountDto
				{
					AccountId = newuser.Id,
					AccountNumber = newuser.Accounts.First().AccountNumber,
					Balance = newuser.Accounts.First().Balance,
				}

			};

		}

		public async Task<ServiceResponse<SignedInDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
		{
			try
			{
				var email = loginDto.Email.Trim();
				var password = loginDto.Password.Trim();

				ApplicationUser? user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					throw new AuthenticationException("Invalid login credentials");
				}

				bool verifypasswordResult = await _userManager.CheckPasswordAsync(user, password);
				if (!verifypasswordResult)
				{
					throw new AuthenticationException("Invalid login credentials");
				}
				var result = await CreateAccessTokenAsync(user);
				return new ServiceResponse<SignedInDto>
				{
					StatusCode = HttpStatusCode.OK,
					Data = result
				};

			}
			catch (AuthenticationException ex)
			{
				return new ServiceResponse<SignedInDto>
				{
					StatusCode = HttpStatusCode.Unauthorized,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResponse<SignedInDto>> RefreshAccessTokenAsync(TokenDto request)
		{
			try
			{
				var principal = GetPrincipalFromExpiredToken(request.AccessToken);
				if (principal.Identity is null)
				{
					throw new AuthenticationException("Access has expired");

				}

				string email = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Email).Value;
				var user = await _userManager.FindByEmailAsync(email);
				if (user is null)
				{
					throw new AuthenticationException("Access has expired");

				}
				var tokenData = await _unitOfWork.GetRepository<RefreshToken>().SingleAsync(r => r.Token == request.RefreshToken);
				if (tokenData is null)
					throw new AuthenticationException("Access has expired");

				bool isRefreshTokenIsValid = tokenData.Token != request.RefreshToken || tokenData.ExpiresAt <= DateTime.UtcNow;

				if (isRefreshTokenIsValid)
				{
					throw new AuthenticationException("Access has expired");
				}

				var result = await CreateAccessTokenAsync(user);
				await _unitOfWork.SaveChangesAsync();
				return new ServiceResponse<SignedInDto>
				{
					StatusCode = HttpStatusCode.OK,
					Data = result
				};
			}
			catch (AuthenticationException ex)
			{
				return new ServiceResponse<SignedInDto>
				{
					StatusCode = HttpStatusCode.Unauthorized,
					Message = ex.Message
				};
			}
		}

		public async Task<SignedInDto> CreateAccessTokenAsync(ApplicationUser? user)
		{
			if (user == null)
			{
				throw new AuthenticationException("Invalid login credentials");
			}

			JwtSecurityTokenHandler jwTokenHandler = new JwtSecurityTokenHandler();
			string? secretKey = _jwtConfig.JwtKey;
			byte[] key = Encoding.ASCII.GetBytes(secretKey);

			var claims = new List<Claim>
			{
				new Claim("UserId", user.Id),
				new Claim(ClaimTypes.Name, user.UserName!),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
				new Claim(JwtRegisteredClaimNames.Email, user.Email!),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
			};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.JwtExpireMinutes),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var securityToken = jwTokenHandler.CreateToken(tokenDescriptor);
			var accessToken = jwTokenHandler.WriteToken(securityToken);
			var refreshToken = await GenerateRefreshTokenAsync(user.Id);
			//var refreshToken = "nmbfdnmgfjksdg";

			await _unitOfWork.SaveChangesAsync();

			return new SignedInDto(accessToken, refreshToken, tokenDescriptor.Expires.Value.ToTimeStamp());
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			string secretKey = _jwtConfig.JwtKey;
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
				ValidateLifetime = true,
				ValidIssuer = _jwtConfig.JwtIssuer,
				ValidAudience = _jwtConfig.JwtAudience
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
				)
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal;
		}
		private async Task<string> GenerateRefreshTokenAsync(string userId)
		{
			var randomNumber = new byte[32];
			using (var generator = RandomNumberGenerator.Create())
			{
				generator.GetBytes(randomNumber);
				var refreshToken = Convert.ToBase64String(randomNumber);
				IEnumerable<RefreshToken> refreshTokens = await _unitOfWork.GetRepository<RefreshToken>()
																	 .GetQueryableList(x => x.UserId == userId && x.IsActive).ToListAsync();

				foreach (var token in refreshTokens)
				{
					token.IsActive = false;
				}
				_unitOfWork.GetRepository<RefreshToken>().Add(new RefreshToken
				{
					CreatedAt = DateTime.UtcNow,
					UpdateAt = DateTime.UtcNow,
					ExpiresAt = DateTime.UtcNow.Add(RefreshTokenValidity),
					IsActive = true,
					UserId = userId,
					Token = refreshToken
				});
				return refreshToken;
			}
		}
		private async Task<string> GenerateUniqueAccountNumber()
		{
			const string Number = "37";
			Random random = new Random();

			string accountNumber;

			do
			{
				var randomNumber = random.Next(10000000, 99999999);
				accountNumber = Number + randomNumber.ToString();
			}
			while (await _accountRepo.AnyAsync(a => a.AccountNumber == accountNumber));

			return accountNumber;
		}

	}

}
