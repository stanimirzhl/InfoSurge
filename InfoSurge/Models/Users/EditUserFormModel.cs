using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;
using static InfoSurge.Resources.ValidationMessages;
using InfoSurge.Resources;

namespace InfoSurge.Models.Users
{
	public class EditUserFormModel
	{
		[Required(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserEmailRequiredErrorMessage))]
		[EmailAddress(
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserEmailInvalidErrorMessage))]
		public string Email { get; set; }

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
			ErrorMessageResourceName = nameof(UserNameRequiredErrorMessage))]
		[StringLength(UserNameMaxLength,
			MinimumLength = UserNameMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserNameLengthMessage))]
		public string UserName { get; set; }

		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		public string? Password { get; set; }

		[StringLength(UserPasswordMaxLength,
			MinimumLength = UserPasswordMinLength,
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordLengthMessage))]
		[DataType(DataType.Password)]
		[Compare("Password",
			ErrorMessageResourceType = typeof(ValidationMessages),
			ErrorMessageResourceName = nameof(UserPasswordNotMatchErrorMessage))]
		public string? ConfirmPassword { get; set; }

		public List<SelectListItem>? Roles { get; set; } = new List<SelectListItem>();

		public List<string>? SelectedRolesIds { get; set; } = new List<string>();

		public List<string>? RoleIdsToRemove { get; set; } = new List<string>();
	}
}
