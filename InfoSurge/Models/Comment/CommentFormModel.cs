using InfoSurge.Resources;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;
using static InfoSurge.Resources.ValidationMessages;

namespace InfoSurge.Models.Comment
{
	public class CommentFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CommentTitleRequiredErrorMessage))]
		[StringLength(CommentTitleMaxLength,
			MinimumLength = CommentTitleMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CommentTitleLengthMessage))]
		public string Title { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CommentContentRequiredErrorMessage))]
		[StringLength(CommentContentMaxLength,
			MinimumLength = CommentContentMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CommentContentLengthMessage))]
		public string Content { get; set; }

		public int ArticleId { get; set; }
	}
}
