
using DemoFintechApp.API.Configurations;
using DemoFintechApp.API.Extensions;
using DemoFintechApp.Logging;
using DemoFintechApp.Services.Utility;
using NLog;

namespace DemoFintechApp.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			IConfiguration configuration = builder.Configuration;
			IServiceCollection services = builder.Services;

			DemoFintechApiConfig DemoFintechApiConfig = configuration.Get<DemoFintechApiConfig>()!;
			services.AddSingleton(DemoFintechApiConfig);
			JwtConfig jwtConfig = DemoFintechApiConfig.JwtConfig;
			services.AddSingleton(jwtConfig);

			LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
		   "/nlog.config"));


			builder.Services.ConfigureSwagger();

			// Add services to the container.
			builder.Services.ConfigureCors();
			builder.Services.ConfigureLoggerService();
			builder.Services.ConfigureSqlContext(builder.Configuration);
			builder.Services.ConfigureAuthentication(jwtConfig);




			builder.Services.AddControllers();

			builder.Services.RegisterServices();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			var logger = app.Services.GetRequiredService<ILoggerService>();
			app.ConfigureExceptionHandler(logger);

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}