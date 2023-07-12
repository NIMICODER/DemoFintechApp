using DemoFintechApp.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoFintechApp.Models.Entities
{
	public class RefreshToken : BaseEntity
	{
		public string UserId { get; set; } = null!;
		public string Token { get; set; } = null!;
		public DateTime ExpiresAt { get; set; }

		[ForeignKey(nameof(UserId))]
		public virtual ApplicationUser User { get; set; } = null!;
	}

}
