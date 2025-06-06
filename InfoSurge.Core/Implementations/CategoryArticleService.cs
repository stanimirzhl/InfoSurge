using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;

namespace InfoSurge.Core.Implementations
{
	public class CategoryArticleService : ICategoryArticleService
	{
		private readonly IRepository<CategoryArticle> repository;

		public CategoryArticleService(IRepository<CategoryArticle> repository)
		{
			this.repository = repository;
		}

		public async Task AddAsync(int id, List<int> categoryIds)
		{
			foreach (int categoryId in categoryIds)
			{
				CategoryArticle categoryArticle = new CategoryArticle()
				{
					ArticleId = id,
					CategoryId = categoryId
				};
				await repository.AddAsync(categoryArticle);
			}
			await repository.SaveChangesAsync();
		}

		public async Task<List<int>> GetSelectedCategories(int articleId)
		{
			return repository
				.AllAsReadOnly()
				.Where(x => x.ArticleId == articleId)
				.Select(x => x.CategoryId)
				.ToList();
		}

		public async Task DeleteAsync(List<int> categoryIds)
		{
			IQueryable<CategoryArticle> categoryArticles = repository
				.All()
				.Where(x => categoryIds.Contains(x.CategoryId));

			await repository.RemoveRange(categoryArticles);
		}
	}
}
