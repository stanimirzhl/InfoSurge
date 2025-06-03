using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.CategoryConstants;

namespace InfoSurge.Data.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MaxLength(CategoryDescriptionMaxLength)]
        public string Description { get; set; }

        public virtual ICollection<CategoryArticle> CategoryArticles { get; set; } = new HashSet<CategoryArticle>();
        public virtual ICollection<CategoryUser> CategoryUsers { get; set; } = new HashSet<CategoryUser>();
    }
}
