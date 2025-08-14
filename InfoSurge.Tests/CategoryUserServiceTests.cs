using InfoSurge.Core;
using InfoSurge.Core.Implementations;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class CategoryUserServiceTests
	{
		private Mock<IRepository<CategoryUser>> mockRepo;
		private CategoryUserService categoryUserService;

		[SetUp]
		public void Setup()
		{
			mockRepo = new Mock<IRepository<CategoryUser>>();
			categoryUserService = new CategoryUserService(mockRepo.Object);
		}

		[Test]
		public async Task IsUserSubscriberToCategory_ShouldReturnTrueIfExists()
		{
			var data = new List<CategoryUser>
			{
				new CategoryUser { CategoryId = 1, UserId = "user1" }
			}.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.All()).Returns(data.Object);

			var result = await categoryUserService.IsUserSubscriberToCategory(1, "user1");

			Assert.That(result);
		}

		[Test]
		public async Task Subscribe_ShouldAddCategoryUser()
		{
			await categoryUserService.Subscribe(2, "user2");

			mockRepo.Verify(r => r.AddAsync(It.Is<CategoryUser>(cu => cu.CategoryId == 2 && cu.UserId == "user2")), Times.Once);
		}

		[Test]
		public async Task UnSubscribe_ShouldDeleteCategoryUser()
		{
			var categoryUser = new CategoryUser { Id = 5, CategoryId = 3, UserId = "user3" };
			var data = new List<CategoryUser> { categoryUser }.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.All()).Returns(data.Object);

			await categoryUserService.UnSubscribe(3, "user3");

			mockRepo.Verify(r => r.DeleteAsync(categoryUser.Id), Times.Once);
		}

		[Test]
		public void UnSubscribe_ShouldThrowExceptionIfNotFound()
		{
			mockRepo.Setup(r => r.All()).Returns(new List<CategoryUser>().AsQueryable().BuildMockDbSet().Object);

			Assert.ThrowsAsync<NoEntityException>(async () => await categoryUserService.UnSubscribe(1, "userX"));
		}

		[Test]
		public async Task GetCategoryIdsByUser_ShouldReturnCorrectIds()
		{
			var data = new List<CategoryUser>
			{
				new CategoryUser { CategoryId = 1, UserId = "user1" },
				new CategoryUser { CategoryId = 2, UserId = "user1" },
				new CategoryUser { CategoryId = 3, UserId = "user2" }
			}.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.All()).Returns(data.Object);

			var result = await categoryUserService.GetCategoryIdsByUser("user1");

			Assert.That(result.Count is 2);
		}

		[Test]
		public async Task GetAllUserEmailsInArticleCategories_ShouldReturnDistinctEmails()
		{
			var data = new List<CategoryUser>
			{
				new CategoryUser { CategoryId = 1, User = new User { Email = "a@test.com" } },
				new CategoryUser { CategoryId = 1, User = new User { Email = "b@test.com" } },
				new CategoryUser { CategoryId = 2, User = new User { Email = "a@test.com" } }
			}.AsQueryable().BuildMockDbSet();

			mockRepo.Setup(r => r.All()).Returns(data.Object);

			var result = await categoryUserService.GetAllUserEmailsInArticleCategories(new List<int> { 1, 2 });

			Assert.That(result.Count is 2);
		}
	}
}
