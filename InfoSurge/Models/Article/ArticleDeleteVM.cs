using InfoSurge.Core.DTOs.ArticleImage;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Models.Article
{
	public class ArticleDeleteVM
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Introduction { get; set; }

		public string Content { get; set; }

		public string MainImageUrl { get; set; }

		public List<ArticleImageDto> AdditionalImages { get; set; } = new List<ArticleImageDto>();

		public List<SelectListItem> CategoryIds { get; set; } = new List<SelectListItem>();

		public List<int> SelectedCategoryIds { get; set; } = new List<int>();
	}
}
