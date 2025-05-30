﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface ICategoryArticleService
    {
        Task AddAsync(int id, List<int> categoryIds);
        Task<List<int>> GetSelectedCategories(int articleId);
        Task DeleteAsync(List<int> categoryIds);
    }
}
