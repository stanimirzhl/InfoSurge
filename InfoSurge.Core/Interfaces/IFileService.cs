using Microsoft.AspNetCore.Http;

namespace InfoSurge.Core.Interfaces
{
    public interface IFileService
    {
        Task<List<string>> GetAdditionalImagesPath(IEnumerable<IFormFile> images);
        Task<string> GetMainImagePath(IFormFile mainImage);
        Task MoveImagesToArticleFolder(int articleId);
        Task<(string, List<string>)> ReplaceMainImageAndAdditionals(int articleId, List<IFormFile> additionalImages, IFormFile? mainImage = null);
        Task DeleteImages(int articleId, List<string> oldAdditionalImages, string? oldMainImage = null);
        Task DeleteImagesFolder(int articleId);
    }
}
