using InfoSurge.Core;
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
                MainImageUrl = null
            };
            return View(formModel);
        }
        [HttpPost]
        public async Task<IActionResult> Add(ArticleFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                formModel.CategoryIds = await categoryService.GetCategoriesIntoSelectList();
                formModel.MainImageUrl = null;
                return View(formModel);
            }

            string mainImagePath = await fileService.GetMainImagePath(formModel.MainImage);

            List<string> additionalImages = new List<string>();
            if (formModel.AdditionalImages.Count != 0)
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

            if (formModel.SelectedCategoryIds.Count != 0)
            {
                await categoryArticleService.AddAsync(articleId, formModel.SelectedCategoryIds);
            }
            await fileService.MoveImagesToArticleFolder(articleId);
            await articleService.ChangeDirectory(articleId);
            await articleImageService.ChangeDirectory(articleId);

            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                ArticleDto articleDto = await articleService.GetByIdAsync(id);

                ArticleFormModel formModel = new ArticleFormModel()
                {
                    Title = articleDto.Title,
                    Introduction = articleDto.Introduction,
                    Content = articleDto.Content,
                    MainImageUrl = articleDto.MainImageUrl,
                    CategoryIds = await categoryService.GetCategoriesIntoSelectList(),
                    SelectedCategoryIds = await categoryArticleService.GetSelectedCategories(id),
                    AdditionalImagesPaths = await articleImageService.GetAllImagePathsById(id),
                };

                return View(formModel);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ArticleFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                formModel.CategoryIds = await categoryService.GetCategoriesIntoSelectList();
                formModel.SelectedCategoryIds = await categoryArticleService.GetSelectedCategories(id);
                formModel.AdditionalImagesPaths = await articleImageService.GetAllImagePathsById(id);
                return View(formModel);
            }

            try
            {
                ArticleDto existingDto = await articleService.GetByIdAsync(id);

                ArticleDto articleDto = new ArticleDto()
                {
                    Id = id,
                    Title = formModel.Title,
                    Introduction = formModel.Introduction,
                    Content = formModel.Content
                };

                if (formModel.MainImage is not null && formModel.MainImage.Length > 0)
                {
                    (string mainImage, List<string> additionalImages) = await fileService.ReplaceMainImageAndAdditionals(id, formModel.AdditionalImages, formModel.MainImage);

                    articleDto.MainImageUrl = mainImage;

                    if (additionalImages.Count > 0)
                    {
                        await articleImageService.AddAsync(id, additionalImages);
                    }

                    await fileService.DeleteImages(id, existingDto.AdditionalImages, existingDto.MainImageUrl);

                    await articleImageService.DeleteAsync(formModel.ImagesIdsToDelete);
                }
                else if (formModel.AdditionalImages.Count > 0)
                {
                    (string empty, List<string> additionalImages) = await fileService.ReplaceMainImageAndAdditionals(id, formModel.AdditionalImages);

                    await articleImageService.AddAsync(id, additionalImages);

                    await fileService.DeleteImages(id, existingDto.AdditionalImages);
                }

                await articleService.EditAsync(articleDto);

                return RedirectToAction("All");
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }

        }
    }
}
