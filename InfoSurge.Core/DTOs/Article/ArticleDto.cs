namespace InfoSurge.Core.DTOs.Article
{
	public class ArticleDto
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Introduction { get; set; }

		public string Content { get; set; }

		public string? AuthorId { get; set; }

		public string? AuthorName { get; set; }

		public string MainImageUrl { get; set; }

		public List<string> AdditionalImages { get; set; } = new List<string>();

		public List<string> CategoryNames { get; set; } = new List<string>();

		public DateTime PublishDate { get; set; }
	}
}
