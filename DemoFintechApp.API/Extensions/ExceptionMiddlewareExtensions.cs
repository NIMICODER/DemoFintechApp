using DemoFintechApp.Logging;
using DemoFintechApp.Models.Utility;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace DemoFintechApp.API.Extensions
{
	public static class ExceptionMiddlewareExtensions
	{
		public static void ConfigureExceptionHandler(this WebApplication app, ILoggerService logger)
		{
			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.ContentType = "application/json";

					IExceptionHandlerFeature? contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						app.Logger.LogError($"{contextFeature.Error}");
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						logger.LogError($"Something went wrong: {contextFeature.Error}");
						string text = JsonSerializer.Serialize(new ErrorResult
						{
							IsSuccessful = false,
							HttpStatusCode = HttpStatusCode.BadRequest,
							//Message = "Unexpected Request Failure",
							Message = contextFeature.Error.Message,
						});
						
						await context.Response.WriteAsync(text);

					}
				});
			});
		}
	}
}
