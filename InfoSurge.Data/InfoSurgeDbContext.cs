using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfoSurge.Data
{
	public class InfoSurgeDbContext : IdentityDbContext<User>
	{
		public InfoSurgeDbContext(DbContextOptions<InfoSurgeDbContext> options) : base(options)
		{

		}

		public virtual DbSet<Category> Categories { get; set; }
		public virtual DbSet<Article> Articles { get; set; }
		public virtual DbSet<Reaction> Reactions { get; set; }
		public virtual DbSet<Comment> Comments { get; set; }
		public virtual DbSet<ArticleImage> ArticleImages { get; set; }
		public virtual DbSet<CategoryArticle> CategoryArticles { get; set; }
		public virtual DbSet<SavedArticle> SavedArticles { get; set; }
		public virtual DbSet<CategoryUser> CategoryUsers { get; set; }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Article>()
				.HasMany(a => a.Reactions)
				.WithOne(r => r.Article)
				.HasForeignKey(r => r.ArticleId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Article>()
				.HasMany(a => a.Comments)
				.WithOne(c => c.Article)
				.HasForeignKey(c => c.ArticleId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Article>()
				.HasMany(x => x.ArticleImages)
				.WithOne(x => x.Article)
				.HasForeignKey(x => x.ArticleId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Reactions)
				.WithOne(r => r.User)
				.HasForeignKey(r => r.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Comments)
				.WithOne(c => c.Author)
				.HasForeignKey(c => c.AuthorId)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<User>()
				.HasMany(x => x.PublishedArticles)
				.WithOne(x => x.Author)
				.HasForeignKey(x => x.AuthorId)
				.OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<CategoryArticle>()
				.HasOne(ca => ca.Category)
				.WithMany(c => c.CategoryArticles)
				.HasForeignKey(ca => ca.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CategoryArticle>()
				.HasOne(ca => ca.Article)
				.WithMany(a => a.CategoryArticles)
				.HasForeignKey(ca => ca.ArticleId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CategoryUser>()
				.HasOne(x => x.User)
				.WithMany(x => x.CategoryUsers)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<CategoryUser>()
				.HasOne(x => x.Category)
				.WithMany(x => x.CategoryUsers)
				.HasForeignKey(x => x.CategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SavedArticle>()
				.HasOne(x => x.User)
				.WithMany(x => x.SavedArticles)
				.HasForeignKey(x => x.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<SavedArticle>()
				.HasOne(x => x.Article)
				.WithMany(x => x.SavedArticles)
				.HasForeignKey(x => x.ArticleId)
				.OnDelete(DeleteBehavior.Cascade);


			base.OnModelCreating(modelBuilder);
		}
	}
}
