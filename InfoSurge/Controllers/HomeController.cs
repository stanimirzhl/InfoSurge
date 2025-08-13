using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models;
using InfoSurge.Models.Article;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InfoSurge.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> logger;
		private readonly IArticleService articleService;
		private readonly ICategoryService categoryService;

		public HomeController(ILogger<HomeController> logger, IArticleService articleService, ICategoryService categoryService)
		{
			this.logger = logger;
			this.articleService = articleService;
			this.categoryService = categoryService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> AllArticles(int pageIndex = 1, ArticleIndexModel categoryAndSearchTermModel = null)
		{

			ViewData["IsEditor"] = User.IsInRole("Editor");

			PagingModel<ArticleDto> pagedArticleDtos = await articleService.GetAllPagedArticles(categoryAndSearchTermModel.SearchTerm, pageIndex, 20, categoryAndSearchTermModel.SelectedCategoryId);

			ArticleIndexModel articleIndex = new ArticleIndexModel()
			{
				PagedArticleModel = await pagedArticleDtos.Map(async x => new ArticleVM()
				{
					Id = x.Id,
					Title = x.Title,
					Introduction = x.Introduction,
					Content = x.Content,
					MainImageUrl = x.MainImageUrl,
					Author = x.AuthorName,
					PublishDate = x.PublishDate.ToString("HH:mm | dd.MM.yy"),
					AdditionalImages = x.AdditionalImages,
					ArticleCategories = x.CategoryNames
				}),
				CategoryIds = await categoryService.GetCategoriesIntoSelectList(),
				SearchTerm = categoryAndSearchTermModel.SearchTerm,
				SelectedCategoryId = categoryAndSearchTermModel.SelectedCategoryId
			};

			return View(articleIndex);
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id, int pageIndex = 1, string? searchTerm = null)
		{
			ViewData["IsEditor"] = User.IsInRole("Editor");

			PagingModel<ArticleDto> pagedArticleDtos = await articleService.GetAllArticlesByCategoryId(id, pageIndex, 20, searchTerm);

			ArticleIndexModel articleIndex = new ArticleIndexModel()
			{
				PagedArticleModel = await pagedArticleDtos.Map(async x => new ArticleVM()
				{
					Id = x.Id,
					Title = x.Title,
					Introduction = x.Introduction,
					Content = x.Content,
					MainImageUrl = x.MainImageUrl,
					Author = x.AuthorName,
					PublishDate = x.PublishDate.ToString("HH:mm | dd.MM.yy"),
					AdditionalImages = x.AdditionalImages,
					ArticleCategories = x.CategoryNames
				}),
				SearchTerm = searchTerm
			};

			return View("Details", articleIndex);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		[Route("Home/Error/{code?}")]
		public IActionResult Error(int? code = null)
		{
			switch (code)
			{
				case 404:
					return View("Error404");
				case 500:
					return View("Error500");
			}

			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
