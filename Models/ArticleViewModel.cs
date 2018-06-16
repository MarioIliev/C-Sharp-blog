namespace CSharpBlog.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ArticleViewModel
    {

        public ArticleViewModel(Article article)
        {
            this.Id = article.Id;
            this.Title = article.Title;
            this.Content = article.Content;
            this.AuthorId = article.AuthorId;
        }

        public ArticleViewModel()
        {

        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Title { get; set; }

        public string Content { get; set; }

        public string AuthorId { get; set; }
    }
}
