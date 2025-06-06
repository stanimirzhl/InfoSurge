using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;
using static InfoSurge.Resources.ValidationMessages;
using InfoSurge.Resources;

namespace InfoSurge.Models.Account
{
	public class LoginFormModel
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
			ErrorMessageResourceName = nameof(UserPasswordRequiredErrorMessage))]
		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public string? ReturnUrl { get; set; }
	}
}
