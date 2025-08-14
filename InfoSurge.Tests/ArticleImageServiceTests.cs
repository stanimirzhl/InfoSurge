using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class ArticleImageServiceTests
	{
		private Mock<IRepository<ArticleImage>> mockRepo;
		private ArticleImageService articleImageService;

		[SetUp]
		public void Setup()
		{
			mockRepo = new Mock<IRepository<ArticleImage>>();
			articleImageService = new ArticleImageService(mockRepo.Object);
		}

		[Test]
		public async Task AddAsync_ShouldAddMultipleImages()
		{
			var images = new List<string> { "img1.png", "img2.png" };

			await articleImageService.AddAsync(1, images);

			mockRepo.Verify(r => r.AddAsync(It.IsAny<ArticleImage>()), Times.Exactly(images.Count));
			mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task GetAllImagePathsById_ShouldReturnCorrectDtos()
		{
			var data = new List<ArticleImage>
			{
				new ArticleImage { Id = 1, ArticleId = 1, ImgUrl = "img1.png" },
				new ArticleImage { Id = 2, ArticleId = 1, ImgUrl = "img2.png" }
			}.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.AllAsReadOnly()).Returns(data.Object);

			var result = await articleImageService.GetAllImagePathsById(1);

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Any(x => x.ImagePath == "img1.png"), Is.True);
			Assert.That(result.Any(x => x.ImagePath == "img2.png"), Is.True);
		}

		[Test]
		public async Task GetImagePathsByTheirIds_ShouldReturnCorrectPaths()
		{
			var data = new List<ArticleImage>
			{
				new ArticleImage { Id = 1, ImgUrl = "img1.png" },
				new ArticleImage { Id = 2, ImgUrl = "img2.png" }
			}.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.AllAsReadOnly()).Returns(data.Object);

			var result = await articleImageService.GetImagePathsByTheirIds(new List<int> { 1, 2 });

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Contains("img1.png"), Is.True);
			Assert.That(result.Contains("img2.png"), Is.True);
		}

		[Test]
		public async Task DeleteAsync_ShouldRemoveCorrectImages()
		{
			var data = new List<ArticleImage>
			{
				new ArticleImage { Id = 1 },
				new ArticleImage { Id = 2 }
			}.AsQueryable();

			mockRepo.Setup(r => r.All()).Returns(data);

			await articleImageService.DeleteAsync(new List<int> { 1, 2 });

			mockRepo.Verify(r => r.RemoveRange(It.IsAny<IQueryable<ArticleImage>>()), Times.Once);
		}
	}
}
