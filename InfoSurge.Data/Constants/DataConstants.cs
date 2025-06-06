namespace InfoSurge.Data.Constants
{
	public static class DataConstants
	{
		public static class CategoryConstants
		{
			public const int CategoryNameMaxLength = 35;
			public const int CategoryNameMinLength = 5;

			public const int CategoryDescriptionMaxLength = 200;
			public const int CategoryDescriptionMinLength = 10;

			/*public const string CategoryNameRequiredErrorMessage = "Името на категорията е задължително!";
			public const string CategoryNameLengthMessage = "Дължината на името на категория трябва да е между {2} и {1} символа дълго!";
			
			public const string CategoryDescriptionRequiredErrorMessage = "Описанието на категорията е задължително!";
			public const string CategoryDescriptionLengthMessage = "Дължината на описанието на категория трябва да е между {2} и {1} символа дълго!";*/
		}

		public static class ArticleConstants
		{
			public const int ArticleTitleMaxLength = 100;
			public const int ArticleTitleMinLength = 10;

			public const int ArticleIntroductionMaxLength = 500;
			public const int ArticleIntroductionMinLength = 50;


			public const int ArticleContentMaxLength = 5000;
			public const int ArticleContentMinLength = 100;

			/*public const string ArticleTitleRequiredErrorMessage = "Заглавието на новината е задължително!";
			public const string ArticleTitleLengthMessage = "Дължината на заглавието на новината трябва да е между {2} и {1} символа дълго!";
			
			public const string ArticleIntroductionRequiredErrorMessage = "Въведението на новината е задължително!";
			public const string ArticleIntroductionLengthMessage = "Дължината на въведението на новината трябва да е между {2} и {1} символа дълго!";
			
			public const string ArticleContentRequiredErrorMessage = "Въведението на новината е задължително!";
			public const string ArticleContentLengthMessage = "Дължината на въведението на новината трябва да е между {2} и {1} символа дълго!";
			
			public const string ArticleMainImageErrorMessage = "Основното изображение е задължително!";*/
		}

		public static class UserConstants
		{
			public const int UserFirstNameMaxLength = 25;
			public const int UserFirstNameMinLength = 5;

			public const int UserLastNameMaxLength = 30;
			public const int UserLastNameMinLength = 5;

			public const int UserNameMaxLength = 25;
			public const int UserNameMinLength = 5;

			public const int UserPasswordMaxLength = 25;
			public const int UserPasswordMinLength = 5;

			public enum UserStatus
			{
				Pending = 0,
				Approved = 1
			}

			/*public const string UserFirstNameRequiredErrorMessage = "Името на потребителя е задължително!";
			public const string UserFirstNameLengthMessage = "Дължината на името на потребителя трябва да е между {2} и {1} символа дълго!";
			
			public const string UserLastNameRequiredErrorMessage = "Фамилията на потребителя е задължителна!";
			public const string UserLastNameLengthMessage = "Дължината на фамилията на потребителя трябва да е между {2} и {1} символа дълго!";
			
			public const string UserNameRequiredErrorMessage = "Потребителското име е задължително!";
			public const string UserNameLengthMessage = "Дължината на потребителското име трябва да е между {2} и {1} символа дълго!";
			
			public const string UserPasswordRequiredErrorMessage = "Паролата е задължителна!";
			public const string UserPasswordLengthMessage = "Дължината на паролата трябва да е между {2} и {1} символа дълго!";
			public const string UserPasswordNotMatchErrorMessage = "Паролите не съвпадат!";
			
			public const string UserEmailRequiredErrorMessage = "Имейл адреса е задължителен!";
			public const string UserEmailInvalidErrorMessage = "Имейл адреса не е в правилния формат, моля опитайте отново (формат: 'example@example.com')!";*/
		}

		public static class CommentConstants
		{
			public const int CommentTitleMaxLength = 100;
			public const int CommentTitleMinLength = 5;

			public const int CommentContentMaxLength = 500;
			public const int CommentContentMinLength = 10;

			public enum CommentStatus
			{
				Pending = 0,
				Approved = 1
			}

			/*public const string CommentTitleRequiredErrorMessage = "Заглавието на коментара е задължително!";
			public const string CommentTitleLengthMessage = "Дължината на заглавието на коментара трябва да е между {2} и {1} символа дълго!";

			public const string CommentContentRequiredErrorMessage = "Съдържанието на коментара е задължително!";
			public const string CommentContentLengthMessage = "Дължината на съдържанието на коментара трябва да е между {2} и {1} символа дълго!";*/
		}
	}
}
