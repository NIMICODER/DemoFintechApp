using DemoFintechApp.Models.Utility;
using DemoFintechApp.Services.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace DemoFintechApp.API.Controllers.Shared
{
	[Produces("application/json")]
	public class BaseController : ControllerBase
	{
		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		protected IActionResult ParseResult(ServiceResult response)
		{
			if (response.HttpStatusCode == HttpStatusCode.OK)
			{
				return base.Ok(response);
			}
			else if (response.HttpStatusCode == HttpStatusCode.BadRequest)
			{
				return base.BadRequest(response);
			}
			else
			{
				throw new InvalidOperationException("Unsupported Result Status");
			}
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		protected IActionResult ComputeApiResponse<T>(ServiceResponse<T> serviceResponse) where T : BaseRecord
		{

			switch (serviceResponse.StatusCode)
			{
				case HttpStatusCode.OK:
					var response = new ApiRecordResponse<T>(true, serviceResponse.Message, serviceResponse.Data);
					return Ok(response);

				case HttpStatusCode.Unauthorized:
					response = new ApiRecordResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return Unauthorized(response);

				case HttpStatusCode.NotFound:
					response = new ApiRecordResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return NotFound(response);

				case HttpStatusCode.BadRequest:
					response = new ApiRecordResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return BadRequest(response);
				default:
					throw new ArgumentOutOfRangeException("HTTP Status Code Could Not Be Deciphered", nameof(serviceResponse.StatusCode));

			}
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		protected IActionResult ComputeResponse<T>(ServiceResponse<T> serviceResponse) where T : class
		{
			switch (serviceResponse.StatusCode)
			{
				case HttpStatusCode.OK:
					var response = new ApiResponse<T>(true, serviceResponse.Message, serviceResponse.Data);
					return Ok(response);

				case HttpStatusCode.Unauthorized:
					response = new ApiResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return Unauthorized(response);

				case HttpStatusCode.NotFound:
					response = new ApiResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return NotFound(response);

				case HttpStatusCode.BadRequest:
					response = new ApiResponse<T>(false, serviceResponse.Message, serviceResponse.Data);
					return BadRequest(response);
				default:
					throw new ArgumentOutOfRangeException("HTTP Status Code Could Not Be Deciphered", nameof(serviceResponse.StatusCode));

			}
		}


		[ApiExplorerSettings(IgnoreApi = true)]
		public string GetUserId()
		{
			ClaimsPrincipal user = HttpContext.User;
			string userId = user.FindFirstValue("UserId")!;
			return userId;
		}
	}
}
