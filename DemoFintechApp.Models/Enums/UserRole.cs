using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Models.Enums
{
	public enum UserRole
	{
		Admin = 1,
		User
	}

	public static class GetUserRole
	{
		public static string? GetStringValue(this UserRole userRole)
		{
			return userRole switch
			{
				UserRole.Admin => "admin",
				UserRole.User => "user",
				_ => null
			};
		}
	}
}
