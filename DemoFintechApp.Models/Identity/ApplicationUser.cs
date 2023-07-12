using DemoFintechApp.Models.Entities;
using DemoFintechApp.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Models.Identity
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string? MiddleName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public List<Account> Accounts { get; set; }

	}
}
