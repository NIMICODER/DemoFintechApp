using DemoFintechApp.Services.Utility;

namespace DemoFintechApp.API.Configurations
{
	public static class ConfigurationBinder
	{
		public static IServiceCollection BindConfigurations(this IServiceCollection services, IConfiguration configuration)
		{
			JwtConfig jwt = new JwtConfig();


			configuration.GetSection("Jwt").Bind(jwt);

			services.AddSingleton(jwt);

			return services;
		}
	}
}
