using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoSurge.Core.Implementations
{
	public class SavedArticleService : ISavedArticleService
	{
		private readonly IRepository<SavedArticle> repository;

		public SavedArticleService(IRepository<SavedArticle> repository)
		{
			this.repository = repository;
		}

		public async Task AddAsync(string userId, int articleId)
		{
			SavedArticle savedArticle = new SavedArticle()
			{
				UserId = userId,
				ArticleId = articleId
			};

			await repository.AddAsync(savedArticle);
			await repository.SaveChangesAsync();
		}

		public async Task Remove(string userId, int articleId)
		{
			SavedArticle savedArticle = await repository.All()
				.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

			await repository.DeleteAsync(savedArticle.Id);
		}

		public async Task<List<ArticleDto>> GetSavedArticlesByUserId(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new NoEntityException("Потребителят не е в системата!");
			}

			IQueryable<SavedArticle> savedArticles = repository.All()
				.Where(x => x.UserId == userId)
				.Include(x => x.Article);

			return await savedArticles.Select(x => new ArticleDto()
			{
				Id = x.Article.Id,
				Title = x.Article.Title,
				Introduction = x.Article.Introduction,
				PublishDate = x.Article.PublishDate,
				MainImageUrl = x.Article.MainImageUrl
			})
			.ToListAsync();
		}

		public async Task<bool> HasUserSavedThisArticle(string userId, int articleId)
		{

			return await repository.All()
				.AnyAsync(x => x.UserId == userId && x.ArticleId == articleId);
		}
	}
}
