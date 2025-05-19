using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.DTOs.Category;
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

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            Article article = await repository.GetByIdAsync(id) 
                ?? throw new NoEntityException($"Новина с Id {id} не е намерена!");

            return new ArticleDto
            {
                Title = article.Title,
                Introduction = article.Introduction,
                Content = article.Content,
                MainImageUrl = article.MainImageUrl
            };
        }

        public async Task EditAsync(ArticleDto articleDto)
        {
            Article article = await repository.GetByIdAsync(articleDto.Id)
                ?? throw new NoEntityException($"Новина с Id {articleDto.Id} не е намерена!");

            article.Title = articleDto.Title;
            article.Introduction = articleDto.Introduction;
            article.Content = articleDto.Content;

            await repository.SaveChangesAsync();
        }
    }
}
