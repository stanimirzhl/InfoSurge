using InfoSurge.Core.DTOs.ArticleImage;

namespace InfoSurge.Core.Interfaces
{
	public interface IArticleImageService
	{
		Task AddAsync(int id, List<string> imagePaths);
		Task DeleteAsync(List<int> oldImagesIds);
		Task<List<ArticleImageDto>> GetAllImagePathsById(int articleId);
		Task ChangeDirectory(int articleId);
		Task<List<string>> GetImagePathsByTheirIds(List<int> additionalImageIds);
	}
}
