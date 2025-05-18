using InfoSurge.Core;
using InfoSurge.Core.DTOs.Category;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InfoSurge.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryService categoryService;

        public CategoryController(ICategoryService brandService)
        {
            this.categoryService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> All(int pageIndex = 1, int pageSize = 5)
        {
            PagingModel<CategoryDto> pagedCategories = await categoryService.GetAllPagedCategories(pageIndex, pageSize);

            PagingModel<CategoryVM> pagedViewModels =
                pagedCategories.Map(x => new CategoryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                });

            return View(pagedViewModels);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new CategoryFormModel());
        }
        [HttpPost]
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
    }
}
