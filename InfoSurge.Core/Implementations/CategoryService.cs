using InfoSurge.Core.DTOs.Category;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> repository;

        public CategoryService(IRepository<Category> repository)
        {
            this.repository = repository;
        }

        public async Task<PagingModel<CategoryDto>> GetAllPagedCategories(int pageIndex, int pageSize)
        {
            IQueryable<CategoryDto> allMappedCategories = repository
                .AllAsReadOnly()
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                });

            PagingModel<CategoryDto> pagedCategories =
                await PagingModel<CategoryDto>.CreateAsync(allMappedCategories, pageIndex, pageSize);

            return pagedCategories;
        }

        public async Task AddAsync(CategoryDto categoryDto)
        {
            Category category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await repository.AddAsync(category);
        }

        public async Task EditAsync(CategoryDto categoryDto)
        {
            Category category = await repository.GetByIdAsync(categoryDto.Id)
                ?? throw new NoEntityException($"Категория с Id {categoryDto.Id} не е намерена!");

            category.Name = categoryDto.Name;
            category.Description = categoryDto.Description;

            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Category category = await repository.GetByIdAsync(id)
                ?? throw new NoEntityException($"Категория с Id {id} не е намерена!");

            await repository.DeleteAsync(category.Id);
            await repository.SaveChangesAsync();
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            Category category = await repository.GetByIdAsync(id) ?? throw new NoEntityException($"Категория с Id {id} не е намерена!");

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<List<SelectListItem>> GetCategoriesIntoSelectList()
        {
            return await repository
                .AllAsReadOnly()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<CategoryDto>> GetCategoryNamesIntoList()
        {
            return await repository
                .AllAsReadOnly()
                .Select(x => new CategoryDto()
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }
    }
}
