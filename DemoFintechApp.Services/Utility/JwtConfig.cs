using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Services.Utility
{
	public class JwtConfig
	{
		public string JwtKey { get; set; } = null!;
		public string JwtIssuer { get; set; } = null!;
		public string JwtAudience { get; set; } = null!;
		public double JwtExpireMinutes { get; set; }
	}

}
