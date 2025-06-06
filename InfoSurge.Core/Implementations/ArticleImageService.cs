using InfoSurge.Core.DTOs.ArticleImage;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InfoSurge.Core.Implementations
{
	public class ArticleImageService : IArticleImageService
	{
		private readonly IRepository<ArticleImage> repository;

		public ArticleImageService(IRepository<ArticleImage> repository)
		{
			this.repository = repository;
		}

		public async Task AddAsync(int id, List<string> imagePaths)
		{
			foreach (string imagePath in imagePaths)
			{
				ArticleImage articleImage = new ArticleImage()
				{
					ArticleId = id,
					ImgUrl = imagePath
				};

				await repository.AddAsync(articleImage);
			}
			await repository.SaveChangesAsync();
		}

		public async Task ChangeDirectory(int articleId)
		{
			IQueryable<ArticleImage> articleImages = repository.All().Where(x => x.ArticleId == articleId);

			await articleImages.ForEachAsync(x =>
			{
				x.ImgUrl = x.ImgUrl.Replace("TempImages", "ArticleImageFolders");
			});

			await articleImages.ForEachAsync(x =>
			{
				if (!string.IsNullOrEmpty(x.ImgUrl))
				{
					string[] parts = x.ImgUrl.Split('/');
					List<string> updatedParts = new List<string>();

					foreach (string part in parts)
					{
						if (!string.IsNullOrWhiteSpace(part))
						{
							updatedParts.Add(part);
							if (part == "ArticleImageFolders")
							{
								updatedParts.Add($"Article-{articleId}-Images");
							}
						}
					}

					x.ImgUrl = "/" + string.Join("/", updatedParts);
				}
			});

			await repository.SaveChangesAsync();
		}

		public async Task DeleteAsync(List<int> oldImagesIds)
		{
			IQueryable<ArticleImage> articleImages = repository
				.All()
				.Where(x => oldImagesIds.Contains(x.Id));

			await repository.RemoveRange(articleImages);
		}

		public async Task<List<ArticleImageDto>> GetAllImagePathsById(int articleId)
		{
			return await repository
				.AllAsReadOnly()
				.Where(x => x.ArticleId == articleId)
				.Select(x => new ArticleImageDto
				{
					Id = x.Id,
					ImagePath = x.ImgUrl
				})
				.ToListAsync();
		}

		public async Task<List<string>> GetImagePathsByTheirIds(List<int> additionalImageIds)
		{
			IQueryable<ArticleImage> articleImages = repository
				.AllAsReadOnly()
				.Where(x => additionalImageIds.Contains(x.Id));

			return await articleImages
				.Select(x => x.ImgUrl)
				.ToListAsync();
		}
	}
}
