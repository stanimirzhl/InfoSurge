using InfoSurge.Core;
using InfoSurge.Core.DTOs.Category;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using InfoSurge.Models.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryService categoryService;
        private IAccountService accountService;
        private ICategoryUserService categoryUserService;

        public CategoryController(ICategoryService brandService, 
            IAccountService accountService,
            ICategoryUserService categoryUserService)
        {
            this.categoryService = brandService;
            this.accountService = accountService;
            this.categoryUserService = categoryUserService;
        }

        [HttpGet]
        public async Task<IActionResult> All(int pageIndex = 1, int pageSize = 5)
        {
            ViewData["IsEditor"] = User.IsInRole("Editor");
            ViewData["IsSignedIn"] = await accountService.IsUserSignedIn(User);

            List<int> userSubscribedCategoryIds = await categoryUserService.GetCategoryIdsByUser(User.FindFirstValue(ClaimTypes.NameIdentifier));

            PagingModel<CategoryDto> pagedCategories = await categoryService.GetAllPagedCategories(pageIndex, pageSize);

            PagingModel<CategoryVM> pagedViewModels =
                pagedCategories.Map(x => new CategoryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsUserSubscribed = userSubscribedCategoryIds.Contains(x.Id)
                });

            return View(pagedViewModels);
        }

        [HttpGet]
        [Authorize(Roles = "Editor")]
        public IActionResult Add()
        {
            return View(new CategoryFormModel());
        }
        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Add(CategoryFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return View(formModel);
            }

            CategoryDto categoryDto = new CategoryDto
            {
                Name = formModel.Name,
                Description = formModel.Description
            };

            await categoryService.AddAsync(categoryDto);
            return RedirectToAction("All");
        }

        [HttpGet]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                CategoryDto categoryDto = await categoryService.GetByIdAsync(id);

                CategoryFormModel formModel = new CategoryFormModel
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };

                return View(formModel);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Edit(int id, CategoryFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return View(formModel);
            }

            try
            {
                CategoryDto existingDto = await categoryService.GetByIdAsync(id);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }
            CategoryDto categoryDto = new CategoryDto()
            {
                Id = id,
                Name = formModel.Name,
                Description = formModel.Description
            };

            await categoryService.EditAsync(categoryDto);


            return RedirectToAction("All");
        }

        [HttpGet]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                CategoryDto categoryDto = await categoryService.GetByIdAsync(id);

                CategoryDeleteVM categoryDeleteVM = new CategoryDeleteVM
                {
                    Id = id,
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };

                return View(categoryDeleteVM);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }

        }

        [HttpPost]
        [Authorize(Roles = "Editor")]
        public async Task<IActionResult> Delete(int id, CategoryDeleteVM categoryDeleteVM)
        {
            if (id != categoryDeleteVM.Id)
            {
                return BadRequest("Invalid request");
            }
            try
            {
                CategoryDto categoryDto = await categoryService.GetByIdAsync(id);

                await categoryService.DeleteAsync(id);

                return RedirectToAction("All");
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubscribeToCategory(int categoryId, int pageIndex = 1)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool isSubscribed = await categoryUserService.IsUserSubscriberToCategory(categoryId, userId);

            if (!isSubscribed)
            {
                await categoryUserService.Subscribe(categoryId, userId);
                TempData["Success"] = "Успешно се абонирахте за категорията.";
            }
            else
            {
                try
                {
                    await categoryUserService.UnSubscribe(categoryId, userId);
                }
                catch (NoEntityException ex)
                {
                    return BadRequest();
                }
                TempData["Success"] = "Успешно се отбонирахте от категорията.";
            }

            return RedirectToAction("All", new { pageIndex });
        }
    }
}
