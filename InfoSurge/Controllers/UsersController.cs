using InfoSurge.Core;
using InfoSurge.Core.DTOs.User;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using InfoSurge.Models.Account;
using InfoSurge.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfoSurge.Controllers
{
    public class UsersController : Controller
    {
        private IUserService userService;
        private IAccountService accountService;

        public UsersController(IUserService userService, IAccountService accountService)
        {
            this.userService = userService;
            this.accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> All(int pageIndex = 1, int pageSize = 6)
        {
            ViewData["IsAdmin"] = User.IsInRole("Administrator");

            PagingModel<UserDto> userDtos = await userService.GetAllUsersPaged(pageIndex, pageSize);

            PagingModel<UserVM> pagedUsers = userDtos.Map(x => new UserVM
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                Roles = x.Roles,
                FirstName = x.FirstName,
                LastName = x.LastName,
                IsUserInRole = x.Roles.Contains("User"),
                Status = x.Status
            });

            return View(pagedUsers);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();

            return View(new RegisterFormModel
            {
                Roles = roles
            });
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(RegisterFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                return View(formModel);
            }

            if (await accountService.UserNameExists(formModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Потребител с това име вече съществува!");
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                return View(formModel);
            }

            if (await accountService.EmailExists(formModel.Email))
            {
                ModelState.AddModelError(string.Empty, "Потребител с този имейл вече съществува!");
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                return View(formModel);
            }

            User user = new User
            {
                UserName = formModel.UserName,
                Email = formModel.Email,
                FirstName = formModel.FirstName,
                LastName = formModel.LastName,
            };

            IdentityResult result = await accountService.AddAsync(user, formModel.Password);

            if (result.Succeeded)
            {
                if (formModel.SelectedRolesIds.Count > 0)
                {
                    foreach (string roleId in formModel.SelectedRolesIds)
                    {
                        try
                        {
                            string roleName = await userService.GetRoleNameById(roleId);

                            await userService.AddRoleToUser(user, roleName);
                        }
                        catch (NoEntityException ex)
                        {
                            return BadRequest();
                        }
                    }
                }
            }
            return RedirectToAction("All");
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                User user = await accountService.GetCurrentUserById(id);

                RegisterFormModel formModel = new RegisterFormModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = await userService.GetAllRolesIntoSelectList(),
                    SelectedRolesIds = await userService.GetRoleIdsByUser(user)
                };

                return View(formModel);
            }
            catch (NoEntityException ex)
            {

                return Unauthorized();
            }
        }
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(RegisterFormModel formModel, string id)
        {
            if (!ModelState.IsValid)
            {
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
                return View(formModel);
            }

            if (await accountService.UserNameExists(formModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Потребител с това име вече съществува!");
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
                return View(formModel);
            }

            if (await accountService.EmailExists(formModel.Email))
            {
                ModelState.AddModelError(string.Empty, "Потребител с този имейл вече съществува!");
                List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
                formModel.Roles = roles;
                formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
                return View(formModel);
            }

            try
            {
                User user = await accountService.GetCurrentUserById(id);

                await userService.ChangeUserPassword(user, formModel.Password);

                IdentityResult result = await accountService.UpdateProfile(id, formModel.UserName, formModel.FirstName, formModel.LastName, formModel.Email);

                if (result.Succeeded)
                {
                    List<string> originalRoles = await userService.GetRoleIdsByUser(user);

                    List<string> adminChosenRoles = formModel.SelectedRolesIds;

                    List<string> rolesToAdd = adminChosenRoles.Except(originalRoles).ToList();
                    List<string> rolesToRemove = originalRoles.Except(adminChosenRoles).ToList();

                    foreach (string role in rolesToAdd)
                    {
                        await userService.AddRoleToUser(user, role);
                    }
                    await userService.RemoveRolesFromUser(user, rolesToRemove);
                }
                return RedirectToAction("All");
            }
            catch (NoEntityException ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                User user = await accountService.GetCurrentUserById(userId);

                await accountService.Delete(user);

                return RedirectToAction("All");
            }
            catch(NoEntityException ex)
            {
               return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Approve(string userId)
        {
            try
            {
                User user = await accountService.GetCurrentUserById(userId);

                await accountService.UpdateStatus(user);

                await accountService.AddUserRole(user);

                return RedirectToAction("All");
            }
            catch (NoEntityException ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Reject(string userId)
        {
            try
            {
                User user = await accountService.GetCurrentUserById(userId);

                await accountService.Delete(user);

                return RedirectToAction("All");
            }
            catch (NoEntityException ex)
            {
                return BadRequest();
            }
        }
    }
}
