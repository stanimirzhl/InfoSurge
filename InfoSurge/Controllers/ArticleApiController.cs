using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models.API_Article_Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Diagnostics;

namespace InfoSurge.Controllers
{
	[Route("other")]
	public class ArticleApiController : Controller
	{
		private readonly IMemoryCache cache;
		private readonly IJsonLoadService jsonService;
		private readonly ITranslationService translationService;
		private readonly IStringLocalizer<SharedResources> localizer;

		public ArticleApiController(IMemoryCache cache,
			IJsonLoadService jsonService,
			ITranslationService translationService,
			IStringLocalizer<SharedResources> localizer)
		{
			this.cache = cache;
			this.jsonService = jsonService;
			this.translationService = translationService;
			this.localizer = localizer;
		}

		[Route("all")]
		public async Task<IActionResult> Index(int pageIndex = 1, string source = null, string category = null)
		{
			if(cache.TryGetValue("Error", out object value))
			{
				ViewBag.IsLoading = false;

				ViewData["ErrorMessage"] = localizer["ApiError"].Value;

				ArticleApiIndexModel errorModel = new ArticleApiIndexModel
				{
					Articles = new PagingModel<ArticleApiVM>(new List<ArticleApiVM>(), 0, pageIndex, 0)
				};

				return View("~/Views/API Views/Index.cshtml", errorModel);
			}

			if (!cache.TryGetValue("CachedArticles", out List<ArticleDto> articles) || articles == null || !articles.Any())
			{
				ViewBag.IsLoading = true;

				List<(string, string)> defaultCategories = jsonService.LoadValues("ArticleCategories.json");
				List<(string, string)> defaultSources = jsonService.LoadValues("ArticleSources.json");

				ArticleApiIndexModel defaultModel = new ArticleApiIndexModel
				{
					Articles = new PagingModel<ArticleApiVM>(new List<ArticleApiVM>(), 0, 0, 0),
					Categories = defaultCategories.Select(s => new SelectListItem
					{
						Text = s.Item1,
						Value = s.Item2
					}).ToList(),
					Sources = defaultSources.Select(s => new SelectListItem
					{
						Text = s.Item1,
						Value = s.Item2
					}).ToList(),
					SelectedCategory = category,
					SelectedSource = source
				};

				return View("~/Views/API Views/Index.cshtml", defaultModel);
			}

			int pageSize = 10;

			if (!string.IsNullOrEmpty(source))
				articles = articles.Where(a => a.Source == source).ToList();

			if (!string.IsNullOrEmpty(category))
				articles = articles.Where(a => a.Category == category).ToList();

			List<ArticleDto> pagedDtos = articles
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.ToList();

			List<string> titles = pagedDtos.Select(a => a.Title).ToList();
			List<string> intros = pagedDtos.Select(a => a.Introduction).ToList();

			List<string> translatedTitles = await translationService.TranslateListAsync(titles);
			List<string> translatedIntros = await translationService.TranslateListAsync(intros);

			for (int i = 0; i < pagedDtos.Count; i++)
			{
				ArticleDto article = pagedDtos[i];
				article.Title = translatedTitles[i];
				article.Introduction = translatedIntros[i];
			}

			PagingModel<ArticleApiVM> apiVMs = new PagingModel<ArticleApiVM>(
				pagedDtos.Select(x => new ArticleApiVM
				{
					Author = x.AuthorName,
					Title = x.Title,
					Introduction = x.Introduction,
					MainImageUrl = x.MainImageUrl,
					PublishDate = x.PublishDate == DateTime.MinValue ? null : x.PublishDate.ToString("HH:mm | dd.MM.yy"),
					Url = x.Url,
					SourceName = x.SourceName
				}).ToList(),
				articles.Count,
				pageIndex,
				pageSize);

			List<(string, string)> categories = jsonService.LoadValues("ArticleCategories.json");
			List<(string, string)> sources = jsonService.LoadValues("ArticleSources.json");

			ArticleApiIndexModel model = new ArticleApiIndexModel
			{
				Articles = apiVMs,
				Categories = categories.Select(s => new SelectListItem
				{
					Text = s.Item1,
					Value = s.Item2
				}).ToList(),
				Sources = sources.Select(s => new SelectListItem
				{
					Text = s.Item1,
					Value = s.Item2
				}).ToList(),
				SelectedCategory = category,
				SelectedSource = source
			};

			ViewBag.IsLoading = false;
			return View("~/Views/API Views/Index.cshtml", model);
		}
	}
}
