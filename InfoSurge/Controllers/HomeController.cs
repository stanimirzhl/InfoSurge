using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Implementations;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models;
using InfoSurge.Models.Article;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InfoSurge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService articleService;
        private readonly ICategoryService categoryService;

        public HomeController(ILogger<HomeController> logger, IArticleService articleService, ICategoryService categoryService)
        {
            _logger = logger;
            this.articleService = articleService;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1, ArticleIndexModel categoryAndSearchTermModel = null)
        {
            ViewData["IsEditor"] = User.IsInRole("Editor");

            PagingModel<ArticleDto> pagedArticleDtos = await articleService.GetAllPagedArticles(categoryAndSearchTermModel.SearchTerm, pageIndex, 20, categoryAndSearchTermModel.SelectedCategoryId);

            ArticleIndexModel articleIndex = new ArticleIndexModel()
            {
                PagedArticleModel = pagedArticleDtos.Map(x => new ArticleVM()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
