using InfoSurge.Core.DTOs.Comment;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.EntityFrameworkCore;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;

namespace InfoSurge.Core.Implementations
{
	public class CommentService : ICommentService
	{
		private readonly IRepository<Comment> repository;

		public CommentService(IRepository<Comment> repository)
		{
			this.repository = repository;
		}

		public async Task AddAsync(int articleId, CommentDto commentDto)
		{
			Comment comment = new Comment()
			{
				Title = commentDto.Title,
				Content = commentDto.Content,
				ArticleId = articleId,
				AuthorId = commentDto.AuthorId,
				Status = CommentStatus.Pending
			};

			await repository.AddAsync(comment);
		}

		public async Task Approve(int commentId)
		{
			Comment comment = await repository.GetByIdAsync(commentId) ?? throw new NoEntityException($"Коментар с Id {commentId} не съществува!");

			comment.Status = CommentStatus.Approved;

			await repository.SaveChangesAsync();
		}

		public async Task Remove(int commentId)
		{
			Comment comment = await repository.GetByIdAsync(commentId) ?? throw new NoEntityException($"Коментар с Id {commentId} не съществува!");

			await repository.DeleteAsync(comment.Id);
		}

		public async Task<PagingModel<CommentDto>> GetAllActivePaginatedCommentsByArticleId(int articleId, int pageIndex, int pageSize)
		{
			IQueryable<Comment> comments = repository.All()
				.Where(x => x.ArticleId == articleId && x.Status == CommentStatus.Approved)
				.Include(x => x.Author)
				.OrderByDescending(c => c.CreatedOn);

			IQueryable<CommentDto> commentDtos = comments
				.Select(x => new CommentDto()
				{
					Id = x.Id,
					Title = x.Title,
					Content = x.Content,
					CreatedOn = x.CreatedOn,
					AuthorName = x.Author == null ? "Изтрит потребител" : x.Author.UserName
				});

			return await PagingModel<CommentDto>.CreateAsync(commentDtos, pageIndex, pageSize);
		}

		public async Task<PagingModel<CommentDto>> GetAllPendingPagedComments(int pageIndex, int pageSize)
		{
			IQueryable<Comment> comments = repository.All()
				.Where(x => x.Status == CommentStatus.Pending)
				.Include(x => x.Article)
				.Include(x => x.Author)
				.OrderByDescending(c => c.CreatedOn);

			IQueryable<CommentDto> commentDtos = comments
			   .Select(x => new CommentDto()
			   {
				   Id = x.Id,
				   Title = x.Title,
				   Content = x.Content,
				   CreatedOn = x.CreatedOn,
				   ArticleTitle = x.Article.Title,
				   ArticleId = x.ArticleId,
				   AuthorName = x.Author == null ? "Изтрит потребител" : x.Author.UserName
			   });

			return await PagingModel<CommentDto>.CreateAsync(commentDtos, pageIndex, pageSize);
		}

		public async Task<List<string>> GetAllUsersEmailWhoHaveCommentedUnderArticle(int articleId)
		{
			return await repository
				.All()
				.Include(x => x.Author)
				.Where(c => c.ArticleId == articleId)
				.Where(c => c.Author.Email != null)
				.Select(c => c.Author.Email)
				.Distinct()
				.ToListAsync();
		}
	}
}
