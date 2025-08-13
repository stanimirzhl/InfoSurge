namespace InfoSurge.Models.API_Article_Model
{
	public class ArticleApiVM
	{
		public string Title { get; set; }

		public string Introduction { get; set; }

		public string MainImageUrl { get; set; }

		public string PublishDate { get; set; }

		public string Author { get; set; }

		public string Url { get; set; }

		public string? SourceName { get; set; }
	}
}
