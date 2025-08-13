using InfoSurge.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Models.API_Article_Model
{
	public class ArticleApiIndexModel
	{
		public PagingModel<ArticleApiVM> Articles { get; set; }

		public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

		public List<SelectListItem> Sources { get; set; } = new List<SelectListItem>();

		public string SelectedCategory { get; set; }

		public string SelectedSource { get; set; }
	}
}
