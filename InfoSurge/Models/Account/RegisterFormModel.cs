using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Models.Account
{
    public class RegisterFormModel
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

        [Required(ErrorMessage = UserPasswordRequiredErrorMessage)]
        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = UserPasswordRequiredErrorMessage)]
        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = UserPasswordNotMatchErrorMessage)]
        public string ConfirmPassword { get; set; }
    }
}
