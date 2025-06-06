using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.ArticleConstants;

namespace InfoSurge.Data.Models
{
	public class Article
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(ArticleTitleMaxLength)]
		public string Title { get; set; }

		[Required]
		[MaxLength(ArticleIntroductionMaxLength)]
		public string Introduction { get; set; }

		[Required]
		[MaxLength(ArticleContentMaxLength)]
		public string Content { get; set; }

		public string? AuthorId { get; set; }
		public virtual User? Author { get; set; }

		[Required]
		public string MainImageUrl { get; set; }

		[Required]
		public DateTime PublishDate { get; set; } = DateTime.Now;

		public virtual ICollection<CategoryArticle> CategoryArticles { get; set; } = new HashSet<CategoryArticle>();
		public virtual ICollection<ArticleImage> ArticleImages { get; set; } = new HashSet<ArticleImage>();
		public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
		public virtual ICollection<Reaction> Reactions { get; set; } = new HashSet<Reaction>();
		public virtual ICollection<SavedArticle> SavedArticles { get; set; } = new HashSet<SavedArticle>();
	}
}
