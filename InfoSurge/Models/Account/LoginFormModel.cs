using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Models.Account
{
    public class LoginFormModel
    {
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

        public string? ReturnUrl { get; set; }
    }
}
