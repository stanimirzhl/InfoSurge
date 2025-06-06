namespace InfoSurge.Core.Interfaces
{
	public interface ICategoryUserService
	{
		Task<bool> IsUserSubscriberToCategory(int categoryId, string userId);

		Task Subscribe(int categoryId, string userId);

		Task UnSubscribe(int categoryId, string userId);

		Task<List<int>> GetCategoryIdsByUser(string userId);

		Task<List<string>> GetAllUserEmailsInArticleCategories(List<int> categoryIds);
	}
}
