namespace InfoSurge.Core.DTOs.Comment
{
	public class CommentDto
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Content { get; set; }

		public DateTime CreatedOn { get; set; }

		public string AuthorName { get; set; }

		public string AuthorId { get; set; }

		public string? ArticleTitle { get; set; }

		public int? ArticleId { get; set; }
	}
}
