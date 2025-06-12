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
		}

		public static class ArticleConstants
		{
			public const int ArticleTitleMaxLength = 100;
			public const int ArticleTitleMinLength = 10;

			public const int ArticleIntroductionMaxLength = 500;
			public const int ArticleIntroductionMinLength = 50;


			public const int ArticleContentMaxLength = 5000;
			public const int ArticleContentMinLength = 100;
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
		}
	}
}
