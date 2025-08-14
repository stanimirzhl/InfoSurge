using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
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
	public class SavedArticleServiceTests
	{
		private Mock<IRepository<SavedArticle>> repoMock;
		private SavedArticleService savedArticleService;

		[SetUp]
		public void Setup()
		{
			repoMock = new Mock<IRepository<SavedArticle>>();
			savedArticleService = new SavedArticleService(repoMock.Object);
		}

		[Test]
		public async Task AddAsync_ShouldCallAddAndSaveChanges()
		{
			SavedArticle captured = null;
			repoMock.Setup(r => r.AddAsync(It.IsAny<SavedArticle>()))
					 .Returns(Task.CompletedTask)
					 .Callback<SavedArticle>(sa => captured = sa);

			await savedArticleService.AddAsync("user1", 10);

			Assert.That(captured.UserId, Is.EqualTo("user1"));
			Assert.That(captured.ArticleId, Is.EqualTo(10));
		}

		[Test]
		public async Task Remove_ShouldCallDeleteAsync()
		{
			var savedArticle = new SavedArticle { Id = 1, UserId = "user1", ArticleId = 10 };
			var queryable = new List<SavedArticle> { savedArticle }.AsQueryable().BuildMockDbSet();
			repoMock.Setup(r => r.All()).Returns(queryable.Object);

			await savedArticleService.Remove("user1", 10);

			repoMock.Verify(r => r.DeleteAsync(savedArticle.Id), Times.Once);
		}

		[Test]
		public void GetSavedArticlesByUserId_ShouldThrow_WhenUserIdNull()
		{
			Assert.ThrowsAsync<NoEntityException>(async () =>
				await savedArticleService.GetSavedArticlesByUserId(null));
		}

		[Test]
		public async Task GetSavedArticlesByUserId_ShouldReturnDtos()
		{
			var article = new Article
			{
				Id = 1,
				Title = "Test",
				Introduction = "Intro",
				PublishDate = DateTime.Today,
				MainImageUrl = "image.jpg"
			};

			var savedArticles = new List<SavedArticle>
			{
				new SavedArticle { UserId = "user1", Article = article }
			}.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(savedArticles.Object);

			var result = await savedArticleService.GetSavedArticlesByUserId("user1");

			Assert.That(result.Count, Is.EqualTo(1));
			Assert.That(result[0].Title, Is.EqualTo("Test"));
			Assert.That(result[0], Is.TypeOf<ArticleDto>());
		}

		[Test]
		public async Task HasUserSavedThisArticle_ShouldReturnTrue()
		{
			var queryable = new List<SavedArticle>
			{
				new SavedArticle { UserId = "user1", ArticleId = 10 }
			}.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(queryable.Object);

			var result = await savedArticleService.HasUserSavedThisArticle("user1", 10);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task HasUserSavedThisArticle_ShouldReturnFalse()
		{
			var queryable = new List<SavedArticle>().AsQueryable().BuildMockDbSet();
			repoMock.Setup(r => r.All()).Returns(queryable.Object);

			var result = await savedArticleService.HasUserSavedThisArticle("user1", 10);

			Assert.That(result, Is.False);
		}
	}
}
