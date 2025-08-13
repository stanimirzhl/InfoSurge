using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
	public class ArticleApiService : IArticleApiService
	{
		private readonly ILogger<ArticleApiService> logger;
		private string apiKey;

		public ArticleApiService(ILogger<ArticleApiService> logger, IConfiguration configuration)
		{
			this.logger = logger;
			this.apiKey = configuration["APISettings:ApiKey"];
		}

		public async Task<PagingModel<ArticleDto>> GetPagedArticlesAsync(int pageIndex, int pageSize, List<string> source, string category)
		{
			//try
			//{
			NewsApiClient newsApiClient = new NewsApiClient(apiKey);

			TopHeadlinesRequest request = new TopHeadlinesRequest
			{
				PageSize = pageSize,
				Page = pageIndex,
			};

			if (source != null && source.Count > 0 && source[0] != null)
			{
				request.Sources.AddRange(source);
			}

			if (!string.IsNullOrEmpty(category))
			{
				request.Category = Enum.TryParse<Categories>(category, true, out var parsedCategory) ? parsedCategory : Categories.Business; // fallback to Business
			}

			if ((source == null || source.Count == 0 || source[0] == null) && string.IsNullOrEmpty(category))
			{
				request.Country = Countries.US;
			}

			ArticlesResult articleResult = await newsApiClient.GetTopHeadlinesAsync(request);

			if (articleResult.Status == Statuses.Ok)
			{
				List<ArticleDto> articles = articleResult.Articles
					.Select(article => new ArticleDto
					{
						Title = article.Title ?? "NA",
						Introduction = article.Description ?? "NA",
						MainImageUrl = article.UrlToImage,
						AuthorName = article.Author,
						PublishDate = article.PublishedAt ?? DateTime.MinValue,
						Url = article.Url,
						Source = article.Source.Id,
						SourceName = article.Source.Name,
					})
					.ToList();

				return new PagingModel<ArticleDto>(articles, articleResult.TotalResults, pageIndex, pageSize);
			}
			else
			{
				logger.LogError($"NewsAPI хвърли грешка: {articleResult.Error.Message}");

				throw new Exception($"NewsAPI хвърли грешка: {articleResult.Error.Code}");
			}
			//}
			//catch (Exception ex)
			//{
			//	logger.LogError(ex, "Проблем при извличането на статии от NewsAPI");
			//
			//	return new PagingModel<ArticleDto>(new List<ArticleDto>(), 0, pageIndex, pageSize);
			//}
		}
	}
}
