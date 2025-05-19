using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.DTOs.Category;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

            string[] parts = article.MainImageUrl.Split('/');
            List<string> updatedParts = new List<string>();

            foreach (string part in parts)
            {
                if (!string.IsNullOrEmpty(part))
                {
                    updatedParts.Add(part);
                    if (part == "ArticleImageFolders")
                    {
                        updatedParts.Add($"Article-{articleId}-Images");
                    }
                }
            }
            string newPath = "/" + string.Join("/", updatedParts);

            article.MainImageUrl = newPath;

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

            if (!string.IsNullOrEmpty(articleDto.MainImageUrl))
            {
                article.MainImageUrl = articleDto.MainImageUrl;
            }

            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Article article = await repository.GetByIdAsync(id)
                ?? throw new NoEntityException($"Новина с Id {id} не е намерена!");

            await repository.DeleteAsync(article.Id);
            await repository.SaveChangesAsync();
        }
    }
}
