using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
