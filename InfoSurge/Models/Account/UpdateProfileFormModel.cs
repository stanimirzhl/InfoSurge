using InfoSurge.Resources;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;
using static InfoSurge.Resources.ValidationMessages;

namespace InfoSurge.Models.Account
{
	public class UpdateProfileFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserNameRequiredErrorMessage))]
		[StringLength(UserNameMaxLength,
			MinimumLength = UserNameMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserNameLengthMessage))]
		public string UserName { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserFirstNameRequiredErrorMessage))]
		[StringLength(UserFirstNameMaxLength,
			MinimumLength = UserFirstNameMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserFirstNameLengthMessage))]
		public string FirstName { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserLastNameRequiredErrorMessage))]
		[StringLength(UserLastNameMaxLength,
			MinimumLength = UserLastNameMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserLastNameLengthMessage))]
		public string LastName { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserEmailRequiredErrorMessage))]
		[EmailAddress(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserEmailInvalidErrorMessage))]
		public string Email { get; set; }
	}
}
