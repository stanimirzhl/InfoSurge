using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            await repository.SaveChangesAsync();
        }
    }
}
