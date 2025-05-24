using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Models.Users
{
    public class EditUserFormModel
    {
        [Required(ErrorMessage = UserEmailRequiredErrorMessage)]
        [EmailAddress(ErrorMessage = UserEmailInvalidErrorMessage)]
        public string Email { get; set; }

        [Required(ErrorMessage = UserFirstNameRequiredErrorMessage)]
        [StringLength(UserFirstNameMaxLength,
            MinimumLength = UserFirstNameMinLength,
            ErrorMessage = UserFirstNameLengthMessage)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = UserLastNameRequiredErrorMessage)]
        [StringLength(UserLastNameMaxLength,
            MinimumLength = UserLastNameMinLength,
            ErrorMessage = UserLastNameLengthMessage)]
        public string LastName { get; set; }

        [Required(ErrorMessage = UserNameRequiredErrorMessage)]
        [StringLength(UserNameMaxLength,
            MinimumLength = UserNameMinLength,
            ErrorMessage = UserNameLengthMessage)]
        public string UserName { get; set; }

        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = UserPasswordNotMatchErrorMessage)]
        public string? ConfirmPassword { get; set; }

        public List<SelectListItem>? Roles { get; set; } = new List<SelectListItem>();

        public List<string>? SelectedRolesIds { get; set; } = new List<string>();

        public List<string>? RoleIdsToRemove { get; set; } = new List<string>();
    }
}
