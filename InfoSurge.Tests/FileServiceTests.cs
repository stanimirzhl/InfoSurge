using InfoSurge.Core.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Tests
{

	[TestFixture]
	public class FileServiceTests
	{
		private string _tempRootPath;
		private FileService fileService;

		[SetUp]
		public void SetUp()
		{
			_tempRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(_tempRootPath);

			var envMock = new Mock<IWebHostEnvironment>();
			envMock.Setup(e => e.WebRootPath).Returns(_tempRootPath);

			fileService = new FileService(envMock.Object);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(_tempRootPath))
				Directory.Delete(_tempRootPath, true);
		}

		private IFormFile CreateFakeFile(string fileName, string content = "dummy data")
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
			return new FormFile(stream, 0, stream.Length, "file", fileName)
			{
				Headers = new HeaderDictionary(),
				ContentType = "image/png"
			};
		}

		[Test]
		public async Task GetMainImagePath_CreatesFileAndReturnsPath()
		{
			var file = CreateFakeFile("test.png");

			var path = await fileService.GetMainImagePath(file);

			Assert.That(File.Exists(Path.Combine(_tempRootPath, path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()))));
		}

		[Test]
		public async Task GetAdditionalImagesPath_CreatesFilesAndReturnsPaths()
		{
			var files = new List<IFormFile> { CreateFakeFile("img1.png"), CreateFakeFile("img2.jpg") };

			var paths = await fileService.GetAdditionalImagesPath(files);

			Assert.That(paths.Count, Is.EqualTo(2));
			foreach (var path in paths)
			{
				Assert.That(File.Exists(Path.Combine(_tempRootPath, path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()))));
			}
		}

		[Test]
		public async Task MoveImagesToArticleFolder_MovesFiles()
		{
			var mainImage = CreateFakeFile("main.png");
			var additional = CreateFakeFile("add.png");

			await fileService.GetMainImagePath(mainImage);
			await fileService.GetAdditionalImagesPath(new[] { additional });

			await fileService.MoveImagesToArticleFolder(123);

			var articleFolder = Path.Combine(_tempRootPath, "ArticleImageFolders", "Article-123-Images");
			Assert.That(Directory.Exists(Path.Combine(articleFolder, "MainImage")), Is.True);
			Assert.That(Directory.Exists(Path.Combine(articleFolder, "AdditionalImages")), Is.True);
		}

		[Test]
		public async Task DeleteImages_RemovesSpecifiedFiles()
		{
			var path = await fileService.GetMainImagePath(CreateFakeFile("main.png"));
			var addPaths = await fileService.GetAdditionalImagesPath(new[] { CreateFakeFile("add.png") });

			await fileService.DeleteImages(1, addPaths, path);

			Assert.That(File.Exists(Path.Combine(_tempRootPath, path.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()))), Is.False);
			foreach (var p in addPaths)
			{
				Assert.That(File.Exists(Path.Combine(_tempRootPath, p.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()))), Is.False);
			}
		}

		[Test]
		public async Task DeleteImagesFolder_RemovesFolder()
		{
			await fileService.MoveImagesToArticleFolder(123);
			var folder = Path.Combine(_tempRootPath, "ArticleImageFolders", "Article-123-Images");
			Directory.CreateDirectory(folder);

			await fileService.DeleteImagesFolder(123);

			Assert.That(Directory.Exists(folder), Is.False);
		}
	}
}
