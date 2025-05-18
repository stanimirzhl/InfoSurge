using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            foreach (var imagePath in imagePaths)
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

            await repository.SaveChangesAsync();
        }
    }
}
