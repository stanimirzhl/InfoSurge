using InfoSurge.Core.DTOs.Comment;

namespace InfoSurge.Core.Interfaces
{
    public interface ICommentService
    {
        Task<PagingModel<CommentDto>> GetAllActivePaginatedCommentsByArticleId(int articleId, int pageIndex, int pageSize);

        Task AddAsync(int articleId, CommentDto commentDto);

        Task Approve(int commentId);

        Task Remove(int commentId);

        Task<PagingModel<CommentDto>> GetAllPendingPagedComments(int pageIndex, int pageSize);

        Task<List<string>> GetAllUsersEmailWhoHaveCommentedUnderArticle(int articleId);
    }
}
