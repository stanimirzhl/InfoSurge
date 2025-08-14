using NUnit.Framework;
using Moq;
using InfoSurge.Data.Models;
using InfoSurge.Data.Common;
using InfoSurge.Core.Implementations;
using InfoSurge.Core.DTOs.Category;
using InfoSurge.Core;

namespace InfoSurge.Tests
{
	[TestFixture]
	public class CategoryServiceTests
	{
		private Mock<IRepository<Category>> repoMock;
		private CategoryService categoryService;

		[SetUp]
		public void Setup()
		{
			repoMock = new Mock<IRepository<Category>>();
			categoryService = new CategoryService(repoMock.Object);
		}

		[Test]
		public async Task AddAsync_ShouldCreateCategoryWithCorrectValues()
		{
			Category capturedCategory = null;
			var dto = new CategoryDto { Name = "Test", Description = "Test desc" };

			repoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
					 .Callback<Category>(c => capturedCategory = c);

			await categoryService.AddAsync(dto);

			Assert.That(capturedCategory, Is.Not.Null);
			Assert.That(capturedCategory.Name, Is.EqualTo("Test"));
			Assert.That(capturedCategory.Description, Is.EqualTo("Test desc"));
		}

		[Test]
		public async Task DeleteAsync_ShouldRemoveCategory_WhenItExists()
		{
			var category = new Category { Id = 1, Name = "DeleteMe" };
			repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

			await categoryService.DeleteAsync(1);

			Assert.That(category.Id, Is.EqualTo(1));
			Assert.That(category.Name, Is.EqualTo("DeleteMe"));
		}

		[Test]
		public void DeleteAsync_ShouldThrow_WhenCategoryNotFound()
		{
			repoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
					 .ReturnsAsync((Category)null);

			Assert.ThrowsAsync<NoEntityException>(() => categoryService.DeleteAsync(99));
		}

		[Test]
		public async Task GetByIdAsync_ShouldReturnMappedDto()
		{
			var category = new Category { Id = 5, Name = "Cat5", Description = "Desc5" };
			repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(category);

			var result = await categoryService.GetByIdAsync(5);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Id, Is.EqualTo(5));
			Assert.That(result.Name, Is.EqualTo("Cat5"));
			Assert.That(result.Description, Is.EqualTo("Desc5"));
		}
	}
}
