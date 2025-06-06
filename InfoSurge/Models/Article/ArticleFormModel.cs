using InfoSurge.Core.DTOs.ArticleImage;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.ArticleConstants;
using static InfoSurge.Resources.ValidationMessages;
using InfoSurge.Resources;

namespace InfoSurge.Models.Article
{
	public class ArticleFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleTitleRequiredErrorMessage))]
		[StringLength(ArticleTitleMaxLength, MinimumLength = ArticleTitleMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleTitleLengthMessage))]
		public string Title { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleIntroductionRequiredErrorMessage))]
		[StringLength(ArticleIntroductionMaxLength, MinimumLength = ArticleIntroductionMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleIntroductionLengthMessage))]
		public string Introduction { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleContentRequiredErrorMessage))]
		[StringLength(ArticleContentMaxLength, MinimumLength = ArticleContentMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(ArticleContentLengthMessage))]
		public string Content { get; set; }

		public IFormFile? MainImage { get; set; }

		public List<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();

		public string? MainImageUrl { get; set; }

		public List<SelectListItem> CategoryIds { get; set; } = new List<SelectListItem>();

		public List<int> SelectedCategoryIds { get; set; } = new List<int>();

		public List<int> ImagesIdsToDelete { get; set; } = new List<int>();

		public List<ArticleImageDto> AdditionalImagesPaths { get; set; } = new List<ArticleImageDto>();
	}
}
