using InfoSurge.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InfoSurge.Core.Implementations
{
	public class FileService : IFileService
	{
		private readonly IWebHostEnvironment environment;

		public FileService(IWebHostEnvironment environment)
		{
			this.environment = environment;
		}

		public async Task<List<string>> GetAdditionalImagesPath(IEnumerable<IFormFile> images)
		{
			string tempImageFolder = Path.Combine(environment.WebRootPath, "TempImages");

			if (!Directory.Exists(tempImageFolder))
			{
				Directory.CreateDirectory(tempImageFolder);
			}

			string additionalImagesTempDirectory = Path.Combine(tempImageFolder, "AdditionalImages");

			if (!Directory.Exists(additionalImagesTempDirectory))
			{
				Directory.CreateDirectory(additionalImagesTempDirectory);
			}

			List<string> additionalImagesPath = new List<string>();

			foreach (IFormFile image in images)
			{
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

				string tempFilePath = Path.Combine(additionalImagesTempDirectory, fileName);

				using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
				{
					await image.CopyToAsync(stream);
				}

				string relativePath = Path.Combine("TempImages", "AdditionalImages", fileName).Replace("\\", "/");
				additionalImagesPath.Add("/" + relativePath);
			}

			return additionalImagesPath;
		}

		public async Task<string> GetMainImagePath(IFormFile mainImage)
		{
			string tempImageFolder = Path.Combine(environment.WebRootPath, "TempImages");

			if (!Directory.Exists(tempImageFolder))
			{
				Directory.CreateDirectory(tempImageFolder);
			}

			string mainImageTempDirectory = Path.Combine(tempImageFolder, "MainImage");

			if (!Directory.Exists(mainImageTempDirectory))
			{
				Directory.CreateDirectory(mainImageTempDirectory);
			}

			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);

			string tempFilePath = Path.Combine(mainImageTempDirectory, fileName);

			using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
			{
				await mainImage.CopyToAsync(stream);
			}

			string relativePath = Path.Combine("TempImages", "MainImage", fileName).Replace("\\", "/");

			return $"/{relativePath}";
		}

		public async Task MoveImagesToArticleFolder(int articleId)
		{
			string imagesFolderPath = Path.Combine(environment.WebRootPath, "ArticleImageFolders");

			if (!Directory.Exists(imagesFolderPath))
			{
				Directory.CreateDirectory(imagesFolderPath);
			}

			string tempImageFolderPath = Path.Combine(environment.WebRootPath, "TempImages");
			string tempMainImageFolderPath = Path.Combine(tempImageFolderPath, "MainImage");
			string tempAdditionalImageFolderPath = Path.Combine(tempImageFolderPath, "AdditionalImages");

			string articleFolder = Path.Combine(imagesFolderPath, $"Article-{articleId}-Images");

			if (!Directory.Exists(articleFolder))
			{
				Directory.CreateDirectory(articleFolder);
			}

			string finalMainImageFolder = Path.Combine(articleFolder, "MainImage");
			string finalAdditionalImagesFolder = Path.Combine(articleFolder, "AdditionalImages");

			if (Directory.Exists(tempMainImageFolderPath))
			{
				Directory.Move(tempMainImageFolderPath, finalMainImageFolder);
			}

			if (Directory.Exists(tempAdditionalImageFolderPath))
			{
				Directory.Move(tempAdditionalImageFolderPath, finalAdditionalImagesFolder);
			}
		}

		public async Task<(string, List<string>)> ReplaceMainImageAndAdditionals(int articleId, List<IFormFile> additionalImages, IFormFile? mainImage = null)
		{
			string imagesFolderPath = Path.Combine(environment.WebRootPath, "ArticleImageFolders");

			string articleFolder = Path.Combine(imagesFolderPath, $"Article-{articleId}-Images");

			string mainImageNewPath = string.Empty;
			if (mainImage is not null)
			{
				string mainImageFolder = Path.Combine(articleFolder, "MainImage");
				string fileName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);

				string mainImagePath = Path.Combine(mainImageFolder, fileName);

				using (FileStream stream = new FileStream(mainImagePath, FileMode.Create))
				{
					await mainImage.CopyToAsync(stream);
				}

				mainImageNewPath = $"/{Path.Combine("ArticleImageFolders", $"Article-{articleId}-Images", "MainImage", fileName).Replace("\\", "/")}";
			}
			string additionalImagesFolder = Path.Combine(articleFolder, "AdditionalImages");

			List<string> additionalImagesPath = new List<string>();
			if (additionalImages.Count > 0)
			{
				if (!Directory.Exists(additionalImagesFolder))
				{
					Directory.CreateDirectory(additionalImagesFolder);
				}

				foreach (IFormFile image in additionalImages)
				{
					string imageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

					string tempFilePath = Path.Combine(additionalImagesFolder, imageName);

					using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
					{
						await image.CopyToAsync(stream);
					}
					additionalImagesPath.Add($"/{Path.Combine("ArticleImageFolders", $"Article-{articleId}-Images", "AdditionalImages", imageName).Replace("\\", "/")}");
				}
			}

			return (mainImageNewPath, additionalImagesPath);
		}

		public async Task DeleteImages(int articleId, List<string> oldAdditionalImages, string oldMainImage = null)
		{

			if (oldMainImage is not null)
			{
				string mainImageFolder = $"{environment.WebRootPath}{oldMainImage}";

				if (File.Exists(mainImageFolder))
				{
					File.Delete(mainImageFolder);
				}
			}

			if (oldAdditionalImages.Count > 0)
			{
				foreach (string image in oldAdditionalImages)
				{
					string additionalImageFolder = $"{environment.WebRootPath}{image}";

					if (File.Exists(additionalImageFolder))
					{
						File.Delete(additionalImageFolder);
					}
				}
			}
		}

		public async Task DeleteImagesFolder(int articleId)
		{
			string imagesFolderPath = Path.Combine(environment.WebRootPath, "ArticleImageFolders");

			string articleFolder = Path.Combine(imagesFolderPath, $"Article-{articleId}-Images");

			if (Directory.Exists(articleFolder))
			{
				Directory.Delete(articleFolder, true);
			}
		}
	}
}
