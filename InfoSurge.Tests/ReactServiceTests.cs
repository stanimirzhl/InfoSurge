using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using MockQueryable.Moq;
using Moq;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class ReactServiceTests
	{
		private Mock<IRepository<Reaction>> repoMock;
		private ReactService reactService;

		[SetUp]
		public void SetUp()
		{
			repoMock = new Mock<IRepository<Reaction>>();
			reactService = new ReactService(repoMock.Object);
		}

		[Test]
		public async Task GetAllReactionsForArticle_NoReactions_ReturnsZeroes()
		{
			var reactions = new List<Reaction>().AsQueryable().BuildMockDbSet();
			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.GetAllReactionsForArticle(1);

			Assert.That(result.Item1, Is.EqualTo(0));
			Assert.That(result.Item2, Is.EqualTo(0));
		}

		[Test]
		public async Task GetAllReactionsForArticle_ReturnsCorrectCounts()
		{
			var reactions = new List<Reaction>
			{
				new Reaction { ArticleId = 1, IsLike = true },
				new Reaction { ArticleId = 1, IsLike = true },
				new Reaction { ArticleId = 1, IsLike = false },
				new Reaction { ArticleId = 2, IsLike = true }
            }.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.GetAllReactionsForArticle(1);

			Assert.That(result.Item1, Is.EqualTo(2)); 
			Assert.That(result.Item2, Is.EqualTo(1)); 
		}

		[Test]
		public async Task HasUserReactedToArticle_NoReactions_ReturnsFalse()
		{
			var reactions = new List<Reaction>().AsQueryable().BuildMockDbSet();
			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.HasUserReactedToArticle("user1", 1);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task HasUserReactedToArticle_UserHasReacted_ReturnsTrue()
		{
			var reactions = new List<Reaction>
			{
				new Reaction { UserId = "user1", ArticleId = 1 }
			}.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.HasUserReactedToArticle("user1", 1);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task GetUserReactionToArticle_UserIdNull_ReturnsFalse()
		{
			var result = await reactService.GetUserReactionToArticle(null, 1);

			Assert.That(result, Is.False);
		}

		[Test]
		public async Task GetUserReactionToArticle_NoReaction_ReturnsNull()
		{
			var reactions = new List<Reaction>().AsQueryable().BuildMockDbSet();
			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.GetUserReactionToArticle("user1", 1);

			Assert.That(result, Is.Null);
		}

		[Test]
		public async Task GetUserReactionToArticle_ReturnsCorrectIsLike()
		{
			var reactions = new List<Reaction>
			{
				new Reaction { UserId = "user1", ArticleId = 1, IsLike = true }
			}.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			var result = await reactService.GetUserReactionToArticle("user1", 1);

			Assert.That(result, Is.True);
		}

		[Test]
		public async Task AddAsync_CreatesReaction()
		{
			repoMock.Setup(r => r.AddAsync(It.IsAny<Reaction>()))
					 .Returns(Task.CompletedTask);

			await reactService.AddAsync("user1", 1, true);

			repoMock.Verify(r => r.AddAsync(It.Is<Reaction>(x =>
				x.UserId == "user1" &&
				x.ArticleId == 1 &&
				x.IsLike == true
			)), Times.Once);
		}

		[Test]
		public async Task ChangeReaction_UpdatesIsLike()
		{
			var reaction = new Reaction { UserId = "user1", ArticleId = 1, IsLike = false };
			var reactions = new List<Reaction> { reaction }.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(reactions.Object);

			await reactService.ChangeReaction("user1", 1, true);

			Assert.That(reaction.IsLike, Is.True);
		}

		[Test]
		public async Task Remove_DeletesReaction()
		{
			var reaction = new Reaction { Id = 10, UserId = "user1", ArticleId = 1 };
			var reactions = new List<Reaction> { reaction }.AsQueryable().BuildMockDbSet();

			repoMock.Setup(r => r.All()).Returns(reactions.Object);
			repoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

			await reactService.Remove("user1", 1);

			repoMock.Verify(r => r.DeleteAsync(10), Times.Once);
		}
	}
}
