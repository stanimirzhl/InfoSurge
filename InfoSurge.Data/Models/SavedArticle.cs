using System.ComponentModel.DataAnnotations;

namespace InfoSurge.Data.Models
{
    public class SavedArticle
    {
        public int Id { get; set; }

        [Required]
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
