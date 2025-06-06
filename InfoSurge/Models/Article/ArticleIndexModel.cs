using InfoSurge.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Models.Article
{
	public class ArticleIndexModel
	{
		public PagingModel<ArticleVM> PagedArticleModel { get; set; }

		public List<SelectListItem> CategoryIds { get; set; } = new List<SelectListItem>();

		public string? SearchTerm { get; set; }

		public int? SelectedCategoryId { get; set; }
	}
}
