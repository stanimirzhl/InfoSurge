using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface IReactService
    {
        Task<(int, int)> GetAllReactionsForArticle(int articleId);

        Task<bool> HasUserReactedToArticle(string userId, int articleId);

        Task<bool?> GetUserReactionToArticle(string userId, int articleId);

        Task AddAsync(string userId, int articleId, bool isLike);

        Task ChangeReaction(string userId, int articleId, bool isLike);
    }
}
