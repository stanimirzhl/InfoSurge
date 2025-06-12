using InfoSurge.Resources;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;
using static InfoSurge.Resources.ValidationMessages;

namespace InfoSurge.Models.Account
{
	public class ChangePasswordFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordRequiredErrorMessage))]
		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordRequiredErrorMessage))]
		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; }

		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordRequiredErrorMessage))]
		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		[Compare("NewPassword",
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordNotMatchErrorMessage))]
		public string ConfirmNewPassword { get; set; }
	}
}
