using DemoFintechApp.Models.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoFintechApp.Models.Entities
{
	public class Account : BaseEntity
	{
		public string AccountNumber { get; set; }
		public long Balance { get; set; }
		public string UserId { get; set; }

		[ForeignKey(nameof(UserId))]
		public ApplicationUser User { get; set; }

		public Card Card { get; set; }
	}


}
