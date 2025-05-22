using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Models.Account
{
    public class ChangePasswordFormModel
    {
        [Required(ErrorMessage = UserPasswordRequiredErrorMessage)]
        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = UserPasswordRequiredErrorMessage)]
        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = UserPasswordRequiredErrorMessage)]
        [StringLength(UserPasswordMaxLength,
            MinimumLength = UserPasswordMinLength,
            ErrorMessage = UserPasswordLengthMessage)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = UserPasswordNotMatchErrorMessage)]
        public string ConfirmNewPassword { get; set; }
    }
}
