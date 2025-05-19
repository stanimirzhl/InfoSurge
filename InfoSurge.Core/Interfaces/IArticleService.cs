using InfoSurge.Core.DTOs.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface IArticleService
    {
        Task<int> AddAsync(ArticleDto articleDto);
        Task EditAsync(ArticleDto articleDto);
        Task<ArticleDto> GetByIdAsync(int id);
        Task ChangeDirectory(int articleId);
    }
}
