using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            public const string CategoryNameRequiredErrorMessage = "Името на категорията е задължително!";
            public const string CategoryNameLengthMessage = "Дължината на името на категория трябва да е между {2} и {1} символа дълго!";

            public const string CategoryDescriptionRequiredErrorMessage = "Описанието на категорията е задължително!";
            public const string CategoryDescriptionLengthMessage = "Дължината на описанието на категория трябва да е между {2} и {1} символа дълго!";
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
