﻿using InfoSurge.Core.DTOs.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Core.Interfaces
{
	public interface ICategoryService
	{
		Task<PagingModel<CategoryDto>> GetAllPagedCategories(int pageIndex, int pageSize);
		Task AddAsync(CategoryDto categoryDto);
		Task EditAsync(CategoryDto categoryDto);
		Task<CategoryDto> GetByIdAsync(int id);
		Task DeleteAsync(int id);
		Task<List<SelectListItem>> GetCategoriesIntoSelectList();
		Task<List<CategoryDto>> GetCategoryNamesIntoList();
	}
}
