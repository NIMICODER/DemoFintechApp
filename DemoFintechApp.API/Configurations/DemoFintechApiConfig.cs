using DemoFintechApp.Services.Utility;

namespace DemoFintechApp.API.Configurations
{
	public class DemoFintechApiConfig
	{
		public string ConnectionString { get; set; } = null!;
		public JwtConfig JwtConfig { get; set; } = null!;
	}
}
