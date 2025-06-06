namespace InfoSurge.Core.Interfaces
{
	public interface ICategoryArticleService
	{
		Task AddAsync(int id, List<int> categoryIds);
		Task<List<int>> GetSelectedCategories(int articleId);
		Task DeleteAsync(List<int> categoryIds);
	}
}
