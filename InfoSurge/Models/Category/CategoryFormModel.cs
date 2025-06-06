using InfoSurge.Resources;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CategoryConstants;
using static InfoSurge.Resources.ValidationMessages;

namespace InfoSurge.Models.Category
{
	public class CategoryFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CategoryNameRequiredErrorMessage))]
		[StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CategoryNameLengthMessage))]
		public string Name { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CategoryDescriptionRequiredErrorMessage))]
		[StringLength(CategoryDescriptionMaxLength, MinimumLength = CategoryDescriptionMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(CategoryDescriptionLengthMessage))]
		public string Description { get; set; }
	}
}
