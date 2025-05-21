using InfoSurge.Core;
using InfoSurge.Models.Comment;

namespace InfoSurge.Models.Article
{
    public class ArticleDetailsModel
    {
        public ArticleVM Article { get; set; }

        public PagingModel<CommentVM> PagedComments { get; set; }

        public CommentFormModel Comment { get; set; }
    }
}
