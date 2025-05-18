using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Implementations;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models.Article;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class ArticleController : Controller
    {
        private IArticleService articleService;
        private IArticleImageService articleImageService;
        private IFileService fileService;
        private ICategoryService categoryService;
        private ICategoryArticleService categoryArticleService;

        public ArticleController(IArticleService articleService, IArticleImageService articleImageService, IFileService fileService, ICategoryService categoryService, ICategoryArticleService categoryArticleService)
        {
            this.articleService = articleService;
            this.articleImageService = articleImageService;
            this.fileService = fileService;
            this.categoryService = categoryService;
            this.categoryArticleService = categoryArticleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ArticleFormModel formModel = new ArticleFormModel
            {
                CategoryIds = await categoryService.GetCategoriesIntoSelectList(),
            };
            return View(formModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ArticleFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                formModel.CategoryIds = await categoryService.GetCategoriesIntoSelectList();
                return View(formModel);
            }

            string mainImagePath = await fileService.GetMainImagePath(formModel.MainImage);

            List<string> additionalImages = new List<string>();
            if(formModel.AdditionalImages.Count != 0)
            {
                additionalImages = await fileService.GetAdditionalImagesPath(formModel.AdditionalImages);
            }

            List<int> categoryIds;

            ArticleDto articleDto = new ArticleDto()
            {
                Title = formModel.Title,
                Introduction = formModel.Introduction,
                Content = formModel.Content,
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AdditionalImages = additionalImages,
                MainImageUrl = mainImagePath,
            };
            int articleId = await articleService.AddAsync(articleDto);

            await articleImageService.AddAsync(articleId, additionalImages);

            if(formModel.SelectedCategoryIds.Count != 0)
            {
                await categoryArticleService.AddAsync(articleId, formModel.SelectedCategoryIds);
            }
            await fileService.MoveImagesToArticleFolder(articleId);
            await articleService.ChangeDirectory(articleId);
            await articleImageService.ChangeDirectory(articleId);

            return RedirectToAction("All");
        }
    }
}
