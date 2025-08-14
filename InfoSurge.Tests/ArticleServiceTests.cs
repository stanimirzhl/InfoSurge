using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Moq;
using MockQueryable.Moq;
using Moq.EntityFrameworkCore;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class ArticleServiceTests
	{
		private Mock<IRepository<Article>> repoMock;
		private ArticleService articleService;

		[SetUp]
		public void Setup()
		{
			repoMock = new Mock<IRepository<Article>>();
			articleService = new ArticleService(repoMock.Object);
		}

		[Test]
		public async Task AddAsync_Should_Add_Article_And_Return_Id()
		{
			var dto = new ArticleDto { Title = "Test", Introduction = "Intro", Content = "Content", AuthorId = "123", MainImageUrl = "/TempImages/image.jpg" };
			var article = new Article { Id = 5 };
			Article addedArticle = null;

			repoMock.Setup(r => r.AddAsync(It.IsAny<Article>()))
					.Callback<Article>(a => { a.Id = article.Id; addedArticle = a; })
					.Returns(Task.CompletedTask);

			var result = await articleService.AddAsync(dto);

			Assert.That(result, Is.EqualTo(5));
			Assert.That(addedArticle, Is.Not.Null);
			Assert.That(addedArticle.Title, Is.EqualTo(dto.Title));
			Assert.That(addedArticle.AuthorId, Is.EqualTo(dto.AuthorId));
			Assert.That(addedArticle.MainImageUrl, Is.EqualTo(dto.MainImageUrl));
		}

		[Test]
		public async Task ChangeDirectory_Should_Update_MainImageUrl()
		{
			var article = new Article { Id = 1, MainImageUrl = "/TempImages/image.jpg" };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

			await articleService.ChangeDirectory(1);

			Assert.That(article.MainImageUrl, Does.Contain("ArticleImageFolders"));
			Assert.That(article.MainImageUrl, Does.Contain("Article-1-Images"));
		}

		[Test]
		public void GetByIdAsync_Should_Throw_If_Not_Found()
		{
			repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article)null);

			Assert.ThrowsAsync<NoEntityException>(() => articleService.GetByIdAsync(1));
		}

		[Test]
		public async Task GetByIdAsync_Should_Return_ArticleDto()
		{
			var article = new Article { Id = 1, Title = "T", Introduction = "I", Content = "C", MainImageUrl = "url" };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

			var dto = await articleService.GetByIdAsync(1);

			Assert.That(dto.Title, Is.EqualTo("T"));
			Assert.That(dto.Content, Is.EqualTo("C"));
		}

		[Test]
		public void EditAsync_Should_Throw_If_Not_Found()
		{
			repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article)null);

			Assert.ThrowsAsync<NoEntityException>(() => articleService.EditAsync(new ArticleDto { Id = 5 }));
		}

		[Test]
		public async Task EditAsync_Should_Update_Fields()
		{
			var article = new Article { Id = 5, Title = "Old", Introduction = "OldIntro", Content = "OldContent", MainImageUrl = "old.jpg" };
			var dto = new ArticleDto { Id = 5, Title = "New", Introduction = "NewIntro", Content = "NewContent", MainImageUrl = "new.jpg" };
			repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(article);

			await articleService.EditAsync(dto);

			Assert.That(article.Title, Is.EqualTo("New"));
			Assert.That(article.Introduction, Is.EqualTo("NewIntro"));
			Assert.That(article.Content, Is.EqualTo("NewContent"));
			Assert.That(article.MainImageUrl, Is.EqualTo("new.jpg"));
		}

		[Test]
		public void DeleteAsync_Should_Throw_If_Not_Found()
		{
			repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article)null);

			Assert.ThrowsAsync<NoEntityException>(() => articleService.DeleteAsync(1));
		}

		[Test]
		public async Task DeleteAsync_Should_Delete_And_Save()
		{
			var article = new Article { Id = 1 };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

			await articleService.DeleteAsync(1);

			repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
		}

		[Test]
		public async Task GetAllPagedArticles_Should_Return_PagingModel()
		{
			var articlesList = new List<Article>
			{
				new Article { Id = 1, Title = "A", Introduction = "Intro", PublishDate = DateTime.Now }
			};

			var mockDbSet = articlesList.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.AllAsReadOnly()).Returns(mockDbSet.Object);

			var result = await articleService.GetAllPagedArticles(null, 1, 10, null);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Items.Count, Is.EqualTo(1));
		}

		[Test]
		public async Task GetArticleDetailsById_Should_Return_Details()
		{
			var articles = new List<Article>
			{
				new Article { Id = 1, Title = "Title", Introduction = "Intro", Content = "Content" }
			};

			var mockDbSet = articles.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.AllAsReadOnly()).Returns(mockDbSet.Object);

			var result = await articleService.GetArticleDetailsById(1);

			Assert.That(result.Title, Is.EqualTo("Title"));
		}

		[Test]
		public async Task GetAllArticlesByCategoryId_Should_Filter_Correctly()
		{
			var categoryArticle = new CategoryArticle
			{
				CategoryId = 2,
				Category = new Category { Name = "Cat" }
			};
			var article = new Article
			{
				Id = 1,
				Title = "A",
				Introduction = "Intro",
				PublishDate = DateTime.Now,
				CategoryArticles = new List<CategoryArticle> { categoryArticle }
			};

			var articlesList = new List<Article> { article }.AsQueryable();

			var mockDbSet = articlesList.BuildMockDbSet();

			repoMock.Setup(r => r.AllAsReadOnly()).Returns(mockDbSet.Object);

			var result = await articleService.GetAllArticlesByCategoryId(2, 1, 10, null);

			Assert.That(result.Items.Count, Is.EqualTo(1));
		}
	}
}
