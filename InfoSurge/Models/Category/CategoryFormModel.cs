using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CategoryConstants;

namespace InfoSurge.Models.Category
{
    public class CategoryFormModel
    {
        [Required(ErrorMessage = CategoryNameRequiredErrorMessage)]
        [StringLength(CategoryNameMaxLength, MinimumLength = CategoryNameMinLength, ErrorMessage = CategoryNameLengthMessage)]
        public string Name { get; set; }

        [Required(ErrorMessage = CategoryDescriptionRequiredErrorMessage)]
        [StringLength(CategoryDescriptionMaxLength, MinimumLength = CategoryDescriptionMinLength, ErrorMessage = CategoryDescriptionLengthMessage)]
        public string Description { get; set; }
    }
}
