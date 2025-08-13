using InfoSurge.Core.DTOs.Article;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
	public interface IArticleApiService
	{
		Task<PagingModel<ArticleDto>> GetPagedArticlesAsync(int pageIndex, int pageSize, List<string> source, string category);
	}
}
