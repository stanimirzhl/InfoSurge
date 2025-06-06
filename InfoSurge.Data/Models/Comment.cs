using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;

namespace InfoSurge.Data.Models
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(CommentTitleMaxLength)]
		public string Title { get; set; }

		[Required]
		[MaxLength(CommentContentMaxLength)]
		public string Content { get; set; }

		[Required]
		public DateTime CreatedOn { get; set; } = DateTime.Now;

		[Required]
		public int ArticleId { get; set; }
		public virtual Article Article { get; set; }

		public string? AuthorId { get; set; }
		public virtual User? Author { get; set; }

		public CommentStatus Status { get; set; } = CommentStatus.Pending;
	}
}
