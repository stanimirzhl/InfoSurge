using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Moq;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class CategoryArticleServiceTests
	{
		private Mock<IRepository<CategoryArticle>> repoMock;
		private CategoryArticleService service;

		[SetUp]
		public void SetUp()
		{
			repoMock = new Mock<IRepository<CategoryArticle>>();
			service = new CategoryArticleService(repoMock.Object);
		}

		[Test]
		public async Task AddAsync_Should_Add_CategoryArticles_And_SaveChanges()
		{
			var categoryIds = new List<int> { 1, 2, 3 };
			await service.AddAsync(10, categoryIds);

			foreach (var id in categoryIds)
			{
				repoMock.Verify(r => r.AddAsync(It.Is<CategoryArticle>(c =>
					c.ArticleId == 10 && c.CategoryId == id
				)), Times.Once);
			}

			repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public async Task GetSelectedCategories_Should_Return_Correct_CategoryIds()
		{
			var data = new List<CategoryArticle>
		{
			new CategoryArticle { ArticleId = 1, CategoryId = 100 },
			new CategoryArticle { ArticleId = 1, CategoryId = 101 },
			new CategoryArticle { ArticleId = 2, CategoryId = 102 }
		}.AsQueryable();

			repoMock.Setup(r => r.AllAsReadOnly()).Returns(data);

			var result = await service.GetSelectedCategories(1);

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Contains(100));
			Assert.That(result.Contains(101));
		}

		[Test]
		public async Task DeleteAsync_Should_Call_RemoveRange_And_SaveChanges()
		{
			var data = new List<CategoryArticle>
			{
				new CategoryArticle { ArticleId = 1, CategoryId = 100 },
				new CategoryArticle { ArticleId = 1, CategoryId = 101 }
			}.AsQueryable();

			repoMock.Setup(r => r.All()).Returns(data);

			await service.DeleteAsync(new List<int> { 100, 101 });

			repoMock.Verify(r => r.RemoveRange(It.Is<IQueryable<CategoryArticle>>(q =>
				q.Count() == 2
			)), Times.Once);
		}
	}
}
