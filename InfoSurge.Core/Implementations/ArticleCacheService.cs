using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAPI.Models;
using System.Collections;
using System.Globalization;

namespace InfoSurge.Core.Implementations
{
	public class ArticleCacheService : BackgroundService
	{
		private readonly IServiceProvider serviceProvider;
		private readonly ILogger<ArticleCacheService> logger;
		private const int PageSize = 10;
		//private bool isRunning = true;

		public ArticleCacheService(IServiceProvider serviceProvider, ILogger<ArticleCacheService> logger)
		{
			this.serviceProvider = serviceProvider;
			this.logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await RefreshCache(stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
				await RefreshCache(stoppingToken);
			}
		}

		private async Task RefreshCache(CancellationToken stoppingToken)
		{

			using var scope = serviceProvider.CreateScope();
			var articleApiService = scope.ServiceProvider.GetRequiredService<IArticleApiService>();
			var jsonService = scope.ServiceProvider.GetRequiredService<IJsonLoadService>();
			var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

			try
			{
				logger.LogInformation("Обновяване на статиите от API...");

				List<ArticleDto> allArticles = new List<ArticleDto>();

				allArticles.AddRange(await LoadArticlesAsync(articleApiService, stoppingToken));

				List<string> sources = jsonService.LoadValues("ArticleSources.json").Select(s => s.Item2).ToList();


				foreach (string source in sources)
				{
					allArticles.AddRange(await LoadArticlesAsync(articleApiService, stoppingToken, source: source));
				}

				var categories = jsonService.LoadValues("ArticleCategories.json").Select(c => c.Item2).ToList();


				foreach (var category in categories)
				{
					allArticles.AddRange(await LoadArticlesAsync(articleApiService, stoppingToken, category: category));
				}


				allArticles = allArticles
					.Where(a => !string.IsNullOrWhiteSpace(a.Url))
					.GroupBy(a => a.Url)
					.Select(g => g.First())
					.ToList();

				cache.Set("CachedArticles", allArticles, TimeSpan.FromHours(12));

				logger.LogInformation($"Заредени {allArticles.Count} статии в паметта.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Проблем при записване на статии в паметта");

				cache.Set("Error", string.Empty, TimeSpan.FromHours(12));

				return;
			}
		}

		private async Task<List<ArticleDto>> LoadArticlesAsync(
			IArticleApiService articleApiService,
			CancellationToken stoppingToken,
			string? source = null,
			string? category = null)
		{
			var results = new List<ArticleDto>();
			int pageIndex = 1;

			while (!stoppingToken.IsCancellationRequested)
			{
				var pagedArticles = await articleApiService.GetPagedArticlesAsync(
					pageIndex,
					PageSize,
					new List<string> { source },
					category
				);

				if (pagedArticles.Items == null || pagedArticles.Items.Count == 0)
					break;

				foreach (var article in pagedArticles.Items)
				{
					results.Add(new ArticleDto
					{
						AuthorName = article.AuthorName,
						Title = article.Title,
						Introduction = article.Introduction,
						MainImageUrl = article.MainImageUrl,
						PublishDate = article.PublishDate,
						Url = article.Url,
						Source = source,
						Category = category,
						SourceName = article.SourceName
					});
				}

				if (pagedArticles.TotalPages <= pageIndex)
					break;

				pageIndex++;
			}

			return results;
		}
	}
}
