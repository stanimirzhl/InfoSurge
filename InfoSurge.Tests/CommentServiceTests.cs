using InfoSurge.Core;
using InfoSurge.Core.DTOs.Comment;
using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using MockQueryable.Moq;
using Moq;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class CommentServiceTests
	{
		private Mock<IRepository<Comment>> repoMock;
		private CommentService commentService;

		[SetUp]
		public void SetUp()
		{
			repoMock = new Mock<IRepository<Comment>>();
			commentService = new CommentService(repoMock.Object);
		}

		[Test]
		public async Task AddAsync_Should_Add_Comment()
		{
			var commentDto = new CommentDto
			{
				Title = "Test",
				Content = "Content",
				AuthorId = "user1"
			};

			await commentService.AddAsync(1, commentDto);

			repoMock.Verify(r => r.AddAsync(It.Is<Comment>(c =>
				c.Title == "Test" &&
				c.Content == "Content" &&
				c.ArticleId == 1 &&
				c.AuthorId == "user1" &&
				c.Status == CommentStatus.Pending
			)), Times.Once);
		}

		[Test]
		public async Task Approve_Should_Set_Status_To_Approved()
		{
			var comment = new Comment { Id = 1, Status = CommentStatus.Pending };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

			await commentService.Approve(1);

			Assert.That(comment.Status, Is.EqualTo(CommentStatus.Approved));
			repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
		}

		[Test]
		public void Approve_Should_Throw_If_Comment_Not_Found()
		{
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Comment?)null);

			Assert.ThrowsAsync<NoEntityException>(async () => await commentService.Approve(1));
		}

		[Test]
		public async Task Remove_Should_Delete_Comment()
		{
			var comment = new Comment { Id = 1 };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

			await commentService.Remove(1);

			repoMock.Verify(r => r.DeleteAsync(1), Times.Once);
		}

		[Test]
		public void Remove_Should_Throw_If_Comment_Not_Found()
		{
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Comment?)null);

			Assert.ThrowsAsync<NoEntityException>(async () => await commentService.Remove(1));
		}

		[Test]
		public async Task GetAllUsersEmailWhoHaveCommentedUnderArticle_Should_Return_Emails()
		{
			var comments = new List<Comment>
			{
				new Comment { Author = new User { Email = "a@test.com" }, ArticleId = 1 },
				new Comment { Author = new User { Email = "b@test.com" }, ArticleId = 1 },
				new Comment { Author = new User { Email = null }, ArticleId = 1 }
			}.AsQueryable();

			var mockComments = comments.BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(mockComments.Object);

			var result = await commentService.GetAllUsersEmailWhoHaveCommentedUnderArticle(1);

			Assert.That(result.Count, Is.EqualTo(2));
			Assert.That(result.Contains("a@test.com"));
			Assert.That(result.Contains("b@test.com"));
		}
	}
}
