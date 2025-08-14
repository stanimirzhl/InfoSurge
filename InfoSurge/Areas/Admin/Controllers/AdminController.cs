using InfoSurge.Areas.Admin.Models.Users;
using InfoSurge.Core;
using InfoSurge.Core.DTOs.User;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using InfoSurge.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace InfoSurge.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Administrator")]
	public class AdminController : Controller
	{
		private IUserService userService;
		private IAccountService accountService;
		private IEmailService emailService;
		private readonly IStringLocalizer<SharedResources> localizer;
		private readonly ILogger<AdminController> logger;

		public AdminController(IUserService userService, IAccountService accountService,
			IEmailService emailService,
			IStringLocalizer<SharedResources> localizer,
			ILogger<AdminController> logger)
		{
			this.userService = userService;
			this.accountService = accountService;
			this.emailService = emailService;
			this.localizer = localizer;
			this.logger = logger;
		}

		[HttpGet]
		public IActionResult DashBoard()
		{
			return View("~/Areas/Admin/Views/DashBoard.cshtml");
		}

		[HttpGet]
		public async Task<IActionResult> AllUsers(int pageIndex = 1, int pageSize = 6)
		{
			PagingModel<UserDto> userDtos = await userService.GetAllUsersPaged(pageIndex, pageSize);

			PagingModel<UserVM> pagedUsers = await userDtos.Map(async x => new UserVM
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

			return View("~/Areas/Admin/Views/Users/AllUsers.cshtml", pagedUsers);
		}

		[HttpGet]
		public async Task<IActionResult> Create()
		{
			List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();

			return View("~/Areas/Admin/Views/Users/Create.cshtml", new RegisterFormModel
			{
				Roles = roles
			});
		}
		[HttpPost]
		public async Task<IActionResult> Create(RegisterFormModel formModel)
		{
			if (!ModelState.IsValid)
			{
				List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
				formModel.Roles = roles;
				return View("~/Areas/Admin/Views/Users/Create.cshtml", formModel);
			}

			if (await accountService.UserNameExists(formModel.UserName))
			{
				ModelState.AddModelError(string.Empty, "Потребител с това име вече съществува!");
				List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
				formModel.Roles = roles;
				return View("~/Areas/Admin/Views/Users/Create.cshtml", formModel);
			}

			if (await accountService.EmailExists(formModel.Email))
			{
				ModelState.AddModelError(string.Empty, "Потребител с този имейл вече съществува!");
				List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
				formModel.Roles = roles;
				return View("~/Areas/Admin/Views/Users/Create.cshtml", formModel);
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
				await accountService.UpdateStatus(user);

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
							logger.LogError(ex.Message, ex);

							return BadRequest();
						}
					}
				}
			}
			return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			try
			{
				User user = await accountService.GetCurrentUserById(id);

				EditUserFormModel formModel = new EditUserFormModel
				{
					UserName = user.UserName,
					Email = user.Email,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Roles = await userService.GetAllRolesIntoSelectList(),
					SelectedRolesIds = await userService.GetRoleIdsByUser(user)
				};

				return View("~/Areas/Admin/Views/Users/Edit.cshtml", formModel);
			}
			catch (NoEntityException ex)
			{

				return Unauthorized();
			}
		}
		[HttpPost]
		public async Task<IActionResult> Edit(EditUserFormModel formModel, string id)
		{
			if (!ModelState.IsValid)
			{
				List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
				formModel.Roles = roles;
				formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
				return View("~/Areas/Admin/Views/Users/Edit.cshtml", formModel);
			}

			try
			{
				User user = await accountService.GetCurrentUserById(id);

				if (await accountService.UserNameExists(formModel.UserName) && user.UserName != formModel.UserName)
				{
					ModelState.AddModelError(string.Empty, "Потребител с това име вече съществува!");
					List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
					formModel.Roles = roles;
					formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
					return View("~/Areas/Admin/Views/Users/Edit.cshtml", formModel);
				}

				if (await accountService.EmailExists(formModel.Email) && user.Email != formModel.Email)
				{
					ModelState.AddModelError(string.Empty, "Потребител с този имейл вече съществува!");
					List<SelectListItem> roles = await userService.GetAllRolesIntoSelectList();
					formModel.Roles = roles;
					formModel.SelectedRolesIds = await userService.GetRoleIdsByUser(await accountService.GetCurrentUserById(id));
					return View("~/Areas/Admin/Views/Users/Edit.cshtml", formModel);
				}

				if (!string.IsNullOrEmpty(formModel.Password))
				{
					await userService.ChangeUserPassword(user, formModel.Password);
				}

				IdentityResult result = await accountService.UpdateProfile(id, formModel.UserName, formModel.FirstName, formModel.LastName, formModel.Email);

				if (result.Succeeded)
				{
					List<string> originalRoles = await userService.GetRoleIdsByUser(user);

					List<string> adminChosenRoles = formModel.SelectedRolesIds;

					List<string> rolesToAdd = adminChosenRoles.Except(originalRoles).ToList();
					List<string> rolesToRemove = originalRoles.Except(adminChosenRoles).ToList();

					foreach (string role in rolesToAdd)
					{
						string roleName = await userService.GetRoleNameById(role);

						await userService.AddRoleToUser(user, roleName);
					}

					List<string> roleNamesToRemove = new List<string>();
					foreach (string role in rolesToRemove)
					{
						string roleName = await userService.GetRoleNameById(role);

						roleNamesToRemove.Add(roleName);
					}
					await userService.RemoveRolesFromUser(user, roleNamesToRemove);

					if (rolesToAdd.Count > 0 || rolesToRemove.Count > 0)
					{
						await userService.RefreshSignIn(user);
					}

					TempData["SuccessfulUpdate"] = localizer["UserUpdated"].Value;

					string subject = "Вашите данни бяха променени";
					string message = $@"
                    <p>Здравейте, вашите данни бяха редактирани от нашите администратори, долу ще се появят новите ви данни за вход.</p>
                    <p><strong>Потребителско име:</strong> {user.UserName}</p>
                    <p><strong>Име:</strong> {user.FirstName}.</p>
                    <p><strong>Фамилия:</strong> {user.LastName}.</p>
                    <p><strong>Електронна поща:</strong> {user.Email}.</p>";

					if (!string.IsNullOrEmpty(formModel.Password))
					{
						message += $"<p><strong>Парола:</strong> {formModel.Password}.</p>";
					}

					try
					{
						await emailService.SendEmailAsync(user.Email, subject, message);
					}
					catch (InvalidOperationException ex)
					{
						logger.LogError(ex.Message, ex);
					}
				}
				return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
			}
			catch (NoEntityException ex)
			{
				return BadRequest();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message, ex);

				return RedirectToAction("Error", "Home", new { code = 500 });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string userId)
		{
			string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var strings = localizer.GetAllStrings();

			if (userId == currentUserId)
			{
				TempData["Error"] = localizer["UserDeletion"].Value;
				return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
			}

			try
			{
				User user = await accountService.GetCurrentUserById(userId);

				await accountService.Delete(user);

				return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
			}
			catch (NoEntityException ex)
			{
				return BadRequest();
			}
		}

		[HttpPost]
		public async Task<IActionResult> Approve(string userId)
		{
			try
			{
				User user = await accountService.GetCurrentUserById(userId);

				await accountService.UpdateStatus(user);

				await accountService.AddUserRole(user);

				string subject = "Вие бяхте одобрени";
				string message = $@"
                <h2>Поздравления!</h2>
                <p>Вашето заявление за участие в нашата платформа беше прието.</p>
                <p><strong>Потребителско име:</strong> {user.UserName}</p>
                <p><strong>Име:</strong> {user.FirstName}.</p>
                <p><strong>Фамилия:</strong> {user.LastName}.</p>
                <p><strong>Парола:</strong> Използвайте създадената от вас парола, за да влезете в акаунта си.</p>
                <p>Вече имате възможността да добавяте коментари, реагирате на новини и да се абонирате за избрани от вас категории.</p>
                ";

				try
				{
					await emailService.SendEmailAsync(user.Email, subject, message);
				}
				catch (InvalidOperationException ex)
				{
					logger.LogError(ex.Message, ex);
				}

				return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
			}
			catch (NoEntityException ex)
			{
				return BadRequest();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message, ex);

				return RedirectToAction("Error", "Home", new { code = 500 });
			}
		}

		[HttpPost]
		public async Task<IActionResult> Reject(string userId)
		{
			try
			{
				User user = await accountService.GetCurrentUserById(userId);

				string subject = "Вие не бяхте одобрени";
				string message = $@"
                <p>За съжаление вашето заявление за участие в нашата платформа беше отхвърлено.</p>
                <p><strong>Потребителско име:</strong> {user.UserName}</p>
                <p><strong>Име:</strong> {user.FirstName}.</p>
                <p><strong>Фамилия:</strong> {user.LastName}.</p>
                ";

				try
				{
					await emailService.SendEmailAsync(user.Email, subject, message);
				}
				catch (InvalidOperationException ex)
				{
					logger.LogError(ex.Message, ex);
				}

				await accountService.Delete(user);

				return RedirectToAction("AllUsers", "Admin", new { area = "Admin" });
			}
			catch (NoEntityException ex)
			{
				return BadRequest();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message, ex);

				return RedirectToAction("Error", "Home", new { code = 500 });
			}
		}
	}
}
