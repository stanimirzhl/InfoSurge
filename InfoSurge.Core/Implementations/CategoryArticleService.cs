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
    public class CategoryArticleService : ICategoryArticleService
    {
        private readonly IRepository<CategoryArticle> repository;

        public CategoryArticleService(IRepository<CategoryArticle> repository)
        {
            this.repository = repository;
        }

        public async Task AddAsync(int id, List<int> categoryIds)
        {
            foreach (var categoryId in categoryIds)
            {
                CategoryArticle categoryArticle = new CategoryArticle()
                {
                    ArticleId = id,
                    CategoryId = categoryId
                };
                await repository.AddAsync(categoryArticle);
            }
            await repository.SaveChangesAsync();
        }
    }
}
