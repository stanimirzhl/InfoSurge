using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoSurge.Core.Implementations
{
	public class ArticleService : IArticleService
	{
		private readonly IRepository<Article> repository;

		public ArticleService(IRepository<Article> repository)
		{
			this.repository = repository;
		}

		public async Task<int> AddAsync(ArticleDto articleDto)
		{
			Article article = new Article()
			{
				Title = articleDto.Title,
				Introduction = articleDto.Introduction,
				Content = articleDto.Content,
				AuthorId = articleDto.AuthorId,
				MainImageUrl = articleDto.MainImageUrl
			};

			await repository.AddAsync(article);
			return article.Id;
		}

		public async Task ChangeDirectory(int articleId)
		{
			Article article = await repository.GetByIdAsync(articleId);

			article.MainImageUrl = article.MainImageUrl.Replace("TempImages", "ArticleImageFolders");

			string[] parts = article.MainImageUrl.Split('/');
			List<string> updatedParts = new List<string>();

			foreach (string part in parts)
			{
				if (!string.IsNullOrEmpty(part))
				{
					updatedParts.Add(part);
					if (part == "ArticleImageFolders")
					{
						updatedParts.Add($"Article-{articleId}-Images");
					}
				}
			}
			string newPath = "/" + string.Join("/", updatedParts);

			article.MainImageUrl = newPath;

			await repository.SaveChangesAsync();
		}

		public async Task<ArticleDto> GetByIdAsync(int id)
		{
			Article article = await repository.GetByIdAsync(id)
				?? throw new NoEntityException($"Новина с Id {id} не е намерена!");

			return new ArticleDto
			{
				Title = article.Title,
				Introduction = article.Introduction,
				Content = article.Content,
				MainImageUrl = article.MainImageUrl
			};
		}

		public async Task EditAsync(ArticleDto articleDto)
		{
			Article article = await repository.GetByIdAsync(articleDto.Id)
				?? throw new NoEntityException($"Новина с Id {articleDto.Id} не е намерена!");

			article.Title = articleDto.Title;
			article.Introduction = articleDto.Introduction;
			article.Content = articleDto.Content;

			if (!string.IsNullOrEmpty(articleDto.MainImageUrl))
			{
				article.MainImageUrl = articleDto.MainImageUrl;
			}

			await repository.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			Article article = await repository.GetByIdAsync(id)
				?? throw new NoEntityException($"Новина с Id {id} не е намерена!");

			await repository.DeleteAsync(article.Id);
			await repository.SaveChangesAsync();
		}

		public Task<PagingModel<ArticleDto>> GetAllPagedArticles(string? searchTerm, int pageIndex, int pageSize, int? categoryId)
		{
			IQueryable<Article> articles = repository
				.AllAsReadOnly()
				.Include(x => x.Author)
				.Include(x => x.ArticleImages)
				.Include(x => x.CategoryArticles)
					.ThenInclude(x => x.Category)
				.OrderByDescending(x => x.PublishDate);

			if (categoryId.HasValue && categoryId > 0)
			{
				articles = articles.Where(x => x.CategoryArticles.Any(x => x.CategoryId == categoryId));
			}

			if (!string.IsNullOrEmpty(searchTerm))
			{
				articles = articles.Where(x => x.Title.ToLower().Contains(searchTerm.ToLower())
					|| x.Introduction.ToLower().Contains(searchTerm.ToLower()));
			}

			IQueryable<ArticleDto> allMappedArticles = articles
				.Select(x => new ArticleDto
				{
					Id = x.Id,
					Title = x.Title,
					Introduction = x.Introduction,
					Content = x.Content,
					MainImageUrl = x.MainImageUrl,
					PublishDate = x.PublishDate,
					AuthorName = x.Author == null ? "Изтрит потребител" : (x.Author.FirstName + " " + x.Author.LastName),
					AdditionalImages = x.ArticleImages
					.Select(i => i.ImgUrl)
					.ToList(),
					CategoryNames = x.CategoryArticles
						.Select(c => c.Category.Name)
						.ToList()
				});

			return PagingModel<ArticleDto>.CreateAsync(allMappedArticles, pageIndex, pageSize);
		}

		public async Task<ArticleDto> GetArticleDetailsById(int articleId)
		{
			Article article = await repository
				.AllAsReadOnly()
				.Include(x => x.Author)
				.Include(x => x.ArticleImages)
				.Include(x => x.CategoryArticles)
					.ThenInclude(x => x.Category)
				.FirstOrDefaultAsync(x => x.Id == articleId) ?? throw new NoEntityException($"Новина с Id {articleId} не е намерена!");

			return new ArticleDto
			{
				Id = article.Id,
				Title = article.Title,
				Introduction = article.Introduction,
				Content = article.Content,
				MainImageUrl = article.MainImageUrl,
				PublishDate = article.PublishDate,
				AuthorName = article.Author == null ? "Изтрит потребител" : (article.Author.FirstName + " " + article.Author.LastName),
				AdditionalImages = article.ArticleImages
					.Select(i => i.ImgUrl)
					.ToList(),
				CategoryNames = article.CategoryArticles
					.Select(c => c.Category.Name)
					.ToList()
			};
		}

		public Task<PagingModel<ArticleDto>> GetAllArticlesByCategoryId(int categoryId, int pageIndex, int pageSize, string? searchTerm)
		{
			IQueryable<Article> articles = repository
				.AllAsReadOnly()
				.Include(x => x.Author)
				.Include(x => x.ArticleImages)
				.Include(x => x.CategoryArticles)
					.ThenInclude(x => x.Category)
				.Where(x => x.CategoryArticles.Any(x => x.CategoryId == categoryId))
				.OrderByDescending(x => x.PublishDate);

			if (!string.IsNullOrEmpty(searchTerm))
			{
				articles = articles.Where(x => x.Title.ToLower().Contains(searchTerm.ToLower())
					|| x.Introduction.ToLower().Contains(searchTerm.ToLower()));
			}

			IQueryable<ArticleDto> allMappedArticles = articles
				.Select(x => new ArticleDto
				{
					Id = x.Id,
					Title = x.Title,
					Introduction = x.Introduction,
					Content = x.Content,
					MainImageUrl = x.MainImageUrl,
					PublishDate = x.PublishDate,
					AuthorName = x.Author == null ? "Изтрит потребител" : (x.Author.FirstName + " " + x.Author.LastName),
					AdditionalImages = x.ArticleImages
					.Select(i => i.ImgUrl)
					.ToList(),
					CategoryNames = x.CategoryArticles
						.Select(c => c.Category.Name)
						.ToList()
				});

			return PagingModel<ArticleDto>.CreateAsync(allMappedArticles, pageIndex, pageSize);
		}
	}
}
