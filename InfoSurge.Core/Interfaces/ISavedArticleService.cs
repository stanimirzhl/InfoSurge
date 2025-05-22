using InfoSurge.Core.DTOs.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface ISavedArticleService
    {
        Task<List<ArticleDto>> GetSavedArticlesByUserId(string userId);

        Task<bool> HasUserSavedThisArticle(string userId, int articleId);

        Task AddAsync(string userId, int articleId);

        Task Remove(string userId, int articleId);
    }
}
