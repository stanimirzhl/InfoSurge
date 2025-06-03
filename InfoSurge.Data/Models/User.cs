using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Data.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(UserFirstNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(UserLastNameMaxLength)]
        public string LastName { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Pending;

        public virtual ICollection<Article> PublishedArticles { get; set; } = new HashSet<Article>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<SavedArticle> SavedArticles { get; set; } = new HashSet<SavedArticle>();
        public virtual ICollection<Reaction> Reactions { get; set; } = new HashSet<Reaction>();
        public virtual ICollection<CategoryUser> CategoryUsers { get; set; } = new HashSet<CategoryUser>();
    }
}
