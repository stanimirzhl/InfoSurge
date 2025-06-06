namespace InfoSurge.Core.Interfaces
{
	public interface IReactService
	{
		Task<(int, int)> GetAllReactionsForArticle(int articleId);

		Task<bool> HasUserReactedToArticle(string userId, int articleId);

		Task<bool?> GetUserReactionToArticle(string userId, int articleId);

		Task AddAsync(string userId, int articleId, bool isLike);

		Task ChangeReaction(string userId, int articleId, bool isLike);

		Task Remove(string userId, int articleId);
	}
}
