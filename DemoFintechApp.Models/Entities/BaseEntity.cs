namespace DemoFintechApp.Models.Entities
{
	public class BaseEntity
	{
		public BaseEntity()
		{
			CreatedAt = DateTime.Now;
			UpdateAt = DateTime.Now;
			IsActive = true;
		}

		public Guid Id { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdateAt { get; set; }
	}
}