using InfoSurge.Core.DTOs.Article;

namespace InfoSurge.Core.Interfaces
{
	public interface IArticleService
	{
		Task<PagingModel<ArticleDto>> GetAllPagedArticles(string? searchTerm, int pageIndex, int pageSize, int? categoryId);
		Task<int> AddAsync(ArticleDto articleDto);
		Task EditAsync(ArticleDto articleDto);
		Task<ArticleDto> GetByIdAsync(int id);
		Task DeleteAsync(int id);
		Task ChangeDirectory(int articleId);
		Task<ArticleDto> GetArticleDetailsById(int articleId);
		Task<PagingModel<ArticleDto>> GetAllArticlesByCategoryId(int categoryId, int pageIndex, int pageSize, string? searchTerm);
	}
}
