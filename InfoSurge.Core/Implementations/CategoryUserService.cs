using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
    public class CategoryUserService : ICategoryUserService
    {
        private readonly IRepository<CategoryUser> repository;

        public CategoryUserService(IRepository<CategoryUser> repository)
        {
            this.repository = repository;
        }

        public async Task<bool> IsUserSubscriberToCategory(int categoryId, string userId)
        {
            return await repository
                .All()
                .AnyAsync(cu => cu.CategoryId == categoryId && cu.UserId == userId);
        }

        public async Task Subscribe(int categoryId, string userId)
        {
            CategoryUser categoryUser = new CategoryUser
            {
                CategoryId = categoryId,
                UserId = userId
            };

            await repository.AddAsync(categoryUser);
        }

        public async Task UnSubscribe(int categoryId, string userId)
        {
            CategoryUser categoryUser = await repository
                .All()
                .FirstOrDefaultAsync(cu => cu.CategoryId == categoryId && cu.UserId == userId) ?? throw new NoEntityException("Несъществуваща категория!");

            await repository.DeleteAsync(categoryUser.Id);
        }

        public async Task<List<int>> GetCategoryIdsByUser(string userId)
        {
            return await repository
                .All()
                .Where(cu => cu.UserId == userId)
                .Select(cu => cu.CategoryId)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllUserEmailsInArticleCategories(List<int> categoryIds)
        {
            return await repository
                .All()
                .Include(x => x.User)
                .Where(cu => categoryIds.Contains(cu.CategoryId))
                .Select(cu => cu.User.Email)
                .Distinct()
                .ToListAsync();
        }
    }
}
