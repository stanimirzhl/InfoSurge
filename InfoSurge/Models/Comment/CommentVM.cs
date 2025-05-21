namespace InfoSurge.Models.Comment
{
    public class CommentVM
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string CreatedOn { get; set; }

        public string AuthorName { get; set; }

        public string? ArticleTitle { get; set; }

        public int? ArticleId { get; set; }
    }
}
