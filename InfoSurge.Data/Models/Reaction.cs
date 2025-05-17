using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Data.Models
{
    public class Reaction
    {
        [Key]
        public int Id { get; set; }

        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public bool IsLike { get; set; }
    }
}
