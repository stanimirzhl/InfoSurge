using InfoSurge.Models.Article;

namespace InfoSurge.Models.Account
{
	public class SettingsViewModel
	{
		public UpdateProfileFormModel ProfileFormModel { get; set; }

		public ChangePasswordFormModel PasswordFormModel { get; set; }

		public List<ArticleVM> SavedArticles { get; set; } = new List<ArticleVM>();

		public string Section { get; set; }

		public string UserFullName { get; set; }
	}
}
