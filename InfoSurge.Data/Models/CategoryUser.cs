using System.ComponentModel.DataAnnotations;

namespace InfoSurge.Data.Models
{
	public class CategoryUser
	{
		[Key]
		public int Id { get; set; }

		public int CategoryId { get; set; }
		public virtual Category Category { get; set; }

		public string UserId { get; set; }
		public virtual User User { get; set; }
	}
}
