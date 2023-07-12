using DemoFintechApp.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoFintechApp.Models.Entities
{
	public class Card : BaseEntity
	{
		
		public string CardNumber { get; set; }
		public DateTime ExpiryDate { get; set; }
		public Guid AccountId { get; set; }

		[ForeignKey(nameof(AccountId))]
		public Account Account { get; set; }
		
	}

}
