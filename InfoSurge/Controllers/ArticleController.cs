using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.DTOs.Comment;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models.Article;
using InfoSurge.Models.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		private ICommentService commentService;
		private IAccountService accountService;
		private ISavedArticleService savedArticleService;
		private IReactService reactService;
		private IEmailService emailService;
		private ICategoryUserService categoryUserService;

		public ArticleController(IArticleService articleService,
			IArticleImageService articleImageService,
			IFileService fileService,
			ICategoryService categoryService,
			ICategoryArticleService categoryArticleService,
			ICommentService commentService,
			IAccountService accountService,
			ISavedArticleService savedArticleService,
			IReactService reactService,
			ICategoryUserService categoryUserService,
			IEmailService emailService)
		{
			this.articleService = articleService;
			this.articleImageService = articleImageService;
			this.fileService = fileService;
			this.categoryService = categoryService;
			this.categoryArticleService = categoryArticleService;
			this.commentService = commentService;
			this.accountService = accountService;
			this.savedArticleService = savedArticleService;
			this.reactService = reactService;
			this.emailService = emailService;
			this.categoryUserService = categoryUserService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		[Authorize(Roles = "Editor")]
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
		[Authorize(Roles = "Editor")]
		public async Task<IActionResult> Add(ArticleFormModel formModel)
		{
			if (!ModelState.IsValid)
			{
				formModel.CategoryIds = await categoryService.GetCategoriesIntoSelectList();
				formModel.MainImageUrl = null;
				return View(formModel);
			}
			if (formModel.MainImage is null || formModel.MainImage.Length == 0)
			{
				ModelState.AddModelError("MainImage", "Основното изображение е задължително!");
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

				List<string> userEmails = await categoryUserService.GetAllUserEmailsInArticleCategories(formModel.SelectedCategoryIds);

				foreach (string email in userEmails)
				{
					string message = $@"
                    <h2>Нова статия в категория</h2>
                    <p>Статията <strong>{formModel.Title}</strong> беше публикувана.</p>";

					await emailService.SendEmailAsync(email, "Ново известие", message);
				}

			}
			await fileService.MoveImagesToArticleFolder(articleId);
			await articleService.ChangeDirectory(articleId);
			await articleImageService.ChangeDirectory(articleId);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize(Roles = "Editor")]
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
		[Authorize(Roles = "Editor")]
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
					Content = formModel.Content,
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

					if (formModel.ImagesIdsToDelete.Count > 0)
					{
						List<string> ImagesToRemove = await articleImageService.GetImagePathsByTheirIds(formModel.ImagesIdsToDelete);

						await fileService.DeleteImages(id, ImagesToRemove);

						await articleImageService.DeleteAsync(formModel.ImagesIdsToDelete);
					}

				}
				else if (formModel.ImagesIdsToDelete.Count > 0)
				{
					List<string> ImagesToRemove = await articleImageService.GetImagePathsByTheirIds(formModel.ImagesIdsToDelete);

					await fileService.DeleteImages(id, ImagesToRemove);

					await articleImageService.DeleteAsync(formModel.ImagesIdsToDelete);
				}

				List<int> originalCategoryIds = await categoryArticleService.GetSelectedCategories(id);

				List<int> userChosenIds = formModel.SelectedCategoryIds;

				List<int> categoriesToAdd = userChosenIds.Except(originalCategoryIds).ToList();
				List<int> categoriesToRemove = originalCategoryIds.Except(userChosenIds).ToList();

				await categoryArticleService.AddAsync(id, categoriesToAdd);
				await categoryArticleService.DeleteAsync(categoriesToRemove);

				await articleService.EditAsync(articleDto);

				return RedirectToAction("Index", "Home");
			}
			catch (NoEntityException ex)
			{
				return NotFound();
			}

		}

		[HttpGet]
		[Authorize(Roles = "Editor")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				ArticleDto articleDto = await articleService.GetByIdAsync(id);

				ArticleDeleteVM articleDeleteVM = new ArticleDeleteVM()
				{
					Id = articleDto.Id,
					Title = articleDto.Title,
					Introduction = articleDto.Introduction,
					Content = articleDto.Content,
					MainImageUrl = articleDto.MainImageUrl,
					AdditionalImages = await articleImageService.GetAllImagePathsById(id),
					SelectedCategoryIds = await categoryArticleService.GetSelectedCategories(id),
					CategoryIds = await categoryService.GetCategoriesIntoSelectList(),
				};

				return View(articleDeleteVM);
			}
			catch (NoEntityException ex)
			{
				return NotFound();
			}
		}
		[HttpPost]
		[Authorize(Roles = "Editor")]
		public async Task<IActionResult> Delete(int id, ArticleDeleteVM articleDeleteVM)
		{
			if (id != articleDeleteVM.Id)
			{
				return BadRequest("Invalid request");
			}
			try
			{
				ArticleDto articleDto = await articleService.GetByIdAsync(id);

				await articleService.DeleteAsync(id);

				await fileService.DeleteImagesFolder(id);

				return RedirectToAction("Index", "Home");
			}
			catch (NoEntityException ex)
			{
				return NotFound();
			}
		}

		[HttpGet]
		public async Task<IActionResult> Details(int id, int pageIndex = 1)
		{
			try
			{
				ArticleDto articleDto = await articleService.GetArticleDetailsById(id);

				ArticleVM articleVM = new ArticleVM()
				{
					Id = articleDto.Id,
					Title = articleDto.Title,
					Introduction = articleDto.Introduction,
					Content = articleDto.Content,
					MainImageUrl = articleDto.MainImageUrl,
					AdditionalImages = articleDto.AdditionalImages,
					Author = articleDto.AuthorName,
					PublishDate = articleDto.PublishDate.ToString("f", System.Globalization.CultureInfo.GetCultureInfo("bg-BG")),
					ArticleCategories = articleDto.CategoryNames,
					IsUserSignedIn = await accountService.IsUserSignedIn(User),
					UserHasSavedArticle = await savedArticleService.HasUserSavedThisArticle(User.FindFirstValue(ClaimTypes.NameIdentifier), id),
					HasUserReacted = await reactService.HasUserReactedToArticle(User.FindFirstValue(ClaimTypes.NameIdentifier), id),
					LikeDislikeCount = await reactService.GetAllReactionsForArticle(id),
					UserReaction = await reactService.GetUserReactionToArticle(User.FindFirstValue(ClaimTypes.NameIdentifier), id)
				};

				PagingModel<CommentDto> pagedCommentDto = await commentService.GetAllActivePaginatedCommentsByArticleId(id, pageIndex, 50);

				ArticleDetailsModel articleDetails = new ArticleDetailsModel()
				{
					Article = articleVM,
					PagedComments = pagedCommentDto.Map(x => new CommentVM()
					{
						Id = x.Id,
						Title = x.Title,
						Content = x.Content,
						AuthorName = x.AuthorName,
						CreatedOn = x.CreatedOn.ToString("f", System.Globalization.CultureInfo.GetCultureInfo("bg-BG"))
					}),
					Comment = new CommentFormModel() { ArticleId = id }
				};

				return View(articleDetails);
			}
			catch (NoEntityException ex)
			{
				return NotFound();
			}
		}
	}
}
