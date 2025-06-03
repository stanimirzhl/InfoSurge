using System.ComponentModel.DataAnnotations;

namespace InfoSurge.Data.Models
{
    public class CategoryArticle
    {
        [Key]
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
