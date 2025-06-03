using System.ComponentModel.DataAnnotations;

namespace InfoSurge.Data.Models
{
    public class ArticleImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
