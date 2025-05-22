using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Models.Account
{
    public class UpdateProfileFormModel
    {
        [Required(ErrorMessage = UserNameRequiredErrorMessage)]
        [StringLength(UserNameMaxLength,
            MinimumLength = UserNameMinLength, 
            ErrorMessage = UserNameLengthMessage)]
        public string UserName { get; set; }

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

        [Required(ErrorMessage = UserEmailRequiredErrorMessage)]
        [EmailAddress(ErrorMessage = UserEmailInvalidErrorMessage)]
        public string Email { get; set; }
    }
}
