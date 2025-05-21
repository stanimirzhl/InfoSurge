using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;

namespace InfoSurge.Models.Comment
{
    public class CommentFormModel
    {
        [Required(ErrorMessage = CommentTitleRequiredErrorMessage)]
        [StringLength(CommentTitleMaxLength,
            MinimumLength = CommentTitleMinLength,
            ErrorMessage = CommentTitleLengthMessage)]
        public string Title { get; set; }

        [Required(ErrorMessage = CommentContentRequiredErrorMessage)]
        [StringLength(CommentContentMaxLength, 
            MinimumLength = CommentContentMinLength,
            ErrorMessage = CommentContentLengthMessage)]
        public string Content { get; set; }

        public int ArticleId { get; set; }
    }
}
