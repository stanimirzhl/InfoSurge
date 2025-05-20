using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Models.Article
{
    public class ArticleVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Introduction { get; set; }

        public string Content { get; set; }

        public string MainImageUrl { get; set; }

        public List<string> AdditionalImages { get; set; } = new List<string>();

        public string PublishDate { get; set; }

        public string Author { get; set; }

        public List<string> ArticleCategories { get; set; } = new List<string>();
    }
}
