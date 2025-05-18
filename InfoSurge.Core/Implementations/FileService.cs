using InfoSurge.Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            string additionalImagesTempDirectory = Path.Combine(tempImageFolder, "AdditionalImages");

            if (!Directory.Exists(additionalImagesTempDirectory))
            {
                Directory.CreateDirectory(additionalImagesTempDirectory);
            }

            List<string> additionalImagesPath = new List<string>();

            foreach (var image in images)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                string tempFilePath = Path.Combine(additionalImagesTempDirectory, fileName);

                using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                string relativePath = Path.Combine("TempImages", "AdditionalImages", fileName);
                additionalImagesPath.Add("\\" + relativePath);
            }

            return additionalImagesPath;
        }

        public async Task<string> GetMainImagePath(IFormFile mainImage)
        {
            string tempImageFolder = Path.Combine(environment.WebRootPath, "TempImages");

            string mainImageTempDirectory = Path.Combine(tempImageFolder, "MainImage");

            if (!Directory.Exists(mainImageTempDirectory))
            {
                Directory.CreateDirectory(mainImageTempDirectory);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(mainImage.FileName);

            string tempFilePath = Path.Combine(mainImageTempDirectory, fileName);

            using(FileStream stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await mainImage.CopyToAsync(stream);
            }

            string relativePath = Path.Combine("TempImages", "MainImage", fileName);

            return $"\\{relativePath}";
        }

        public async Task MoveImagesToArticleFolder(int articleId)
        {
            string imagesFolderPath = Path.Combine(environment.WebRootPath, "ArticleImageFolders");

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

    }
}
