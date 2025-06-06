using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoSurge.Core.Implementations
{
	public class ReactService : IReactService
	{
		private readonly IRepository<Reaction> repository;

		public ReactService(IRepository<Reaction> repository)
		{
			this.repository = repository;
		}

		public async Task<(int, int)> GetAllReactionsForArticle(int articleId)
		{
			if (!repository.All().Any())
			{
				return (0, 0);
			}

			IQueryable<Reaction> reactions = repository.All()
				.Where(x => x.ArticleId == articleId);

			int likes = await reactions
				.Where(x => x.IsLike == true)
				.CountAsync();

			int dislikes = await reactions
				.Where(x => x.IsLike == false)
				.CountAsync();

			return (likes, dislikes);
		}

		public async Task<bool> HasUserReactedToArticle(string userId, int articleId)
		{
			if (!repository.All().Any())
			{
				return false;
			}

			Reaction reaction = await repository.All()
				.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

			return reaction != null;
		}

		public async Task<bool?> GetUserReactionToArticle(string userId, int articleId)
		{
			if (userId == null)
			{
				return false;
			}

			Reaction reaction = await repository.All()
				.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

			if (reaction == null)
			{
				return null;
			}

			return reaction.IsLike;
		}

		public async Task AddAsync(string userId, int articleId, bool isLike)
		{

			Reaction reaction = new Reaction()
			{
				UserId = userId,
				ArticleId = articleId,
				IsLike = isLike
			};

			await repository.AddAsync(reaction);
		}

		public async Task ChangeReaction(string userId, int articleId, bool isLike)
		{
			Reaction reaction = repository.All()
				.FirstOrDefault(x => x.UserId == userId && x.ArticleId == articleId);

			reaction.IsLike = isLike;

			await repository.SaveChangesAsync();
		}

		public async Task Remove(string userId, int articleId)
		{
			Reaction reaction = await repository.All()
				.FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

			await repository.DeleteAsync(reaction.Id);
		}
	}
}
