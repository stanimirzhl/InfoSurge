﻿using InfoSurge.Core.DTOs.ArticleImage;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static InfoSurge.Data.Constants.DataConstants.ArticleConstants;

namespace InfoSurge.Models.Article
{
    public class ArticleFormModel
    {
        [Required(ErrorMessage = ArticleTitleRequiredErrorMessage)]
        [StringLength(ArticleTitleMaxLength
            , MinimumLength = ArticleTitleMinLength
            , ErrorMessage = ArticleTitleLengthMessage)]
        public string Title { get; set; }

        [Required(ErrorMessage = ArticleIntroductionRequiredErrorMessage)]
        [StringLength(ArticleIntroductionMaxLength
            , MinimumLength = ArticleIntroductionMinLength
            , ErrorMessage = ArticleIntroductionLengthMessage)]
        public string Introduction { get; set; }

        [Required(ErrorMessage = ArticleContentRequiredErrorMessage)]
        [StringLength(ArticleContentMaxLength, MinimumLength = ArticleContentMinLength, ErrorMessage = ArticleContentLengthMessage)]
        public string Content { get; set; }

        public IFormFile? MainImage { get; set; }

        public List<IFormFile> AdditionalImages { get; set; } = new List<IFormFile>();

        public string? MainImageUrl { get; set; }

        public List<SelectListItem> CategoryIds { get; set; } = new List<SelectListItem>();

        public List<int> SelectedCategoryIds { get; set; } = new List<int>();

        public List<int> ImagesIdsToDelete { get; set; } = new List<int>();

        public List<ArticleImageDto> AdditionalImagesPaths { get; set; } = new List<ArticleImageDto>();
    }
}
