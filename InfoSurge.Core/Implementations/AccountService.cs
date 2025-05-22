using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

namespace InfoSurge.Core.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IdentityResult> AddAsync(User user, string password)
        {
            return await userManager.CreateAsync(user, password);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await userManager.FindByEmailAsync(email) != null;
        }

        public async Task<User> GetCurrentUserById(string userId)
        {
            return await userManager.FindByIdAsync(userId) ?? throw new NoEntityException("Потребителят не е в системата!");
        }

        public async Task<bool> IsPasswordValidForUserName(string userName, string password)
        {
            return await signInManager.CheckPasswordSignInAsync(await signInManager.UserManager.FindByNameAsync(userName), password, false) == SignInResult.Failed;
        }

        public async Task<bool> IsUserApproved(string userName)
        {
            User user = await userManager.FindByNameAsync(userName);

            return user.Status == UserStatus.Approved;
        }

        public async Task<bool> IsUserSignedIn(ClaimsPrincipal principal)
        {
            return signInManager.IsSignedIn(principal);
        }

        public async Task<SignInResult> Login(string userName, string password)
        {
            return await signInManager.PasswordSignInAsync(userName, password, false, false);
        }

        public async Task Logout()
        {
           await signInManager.SignOutAsync();
        }

        public async Task SignInAgain(User user)
        {
            await signInManager.RefreshSignInAsync(user);
        }

        public async Task<IdentityResult> UpdatePassword(User user, string oldPassword, string newPassword)
        {
            return await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateProfile(string userId, string userName, string firstName, string lastName, string email)
        {
            User user = await userManager.FindByIdAsync(userId) ?? throw new NoEntityException("Потребителят не е в системата!");

            user.UserName = userName;
            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;

            return await userManager.UpdateAsync(user);
        }

        public async Task<bool> UserNameExists(string userName)
        {
            return await userManager.FindByNameAsync(userName) != null;
        }
    }
}
