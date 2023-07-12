using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Shared.Dtos.Responses
{
	public record SignedInDto(string AccessToken, string RefreshToken, long ExpiryTimeStamp);
}
