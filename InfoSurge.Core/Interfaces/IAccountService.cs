using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core.Interfaces
{
    public interface IAccountService
    {
        Task<bool> UserNameExists(string userName);

        Task<bool> EmailExists(string email);

        Task<bool> IsPasswordValidForUserName(string userName, string password);

        Task<bool> IsUserApproved(string userName);

        Task<User> GetCurrentUserById(string userId);

        Task<IdentityResult> UpdateProfile(string userId, string userName, string firstName, string lastName, string email);

        Task<IdentityResult> UpdatePassword(User user, string oldPassword, string newPassword);

        Task SignInAgain(User user);

        Task<bool> IsUserSignedIn(ClaimsPrincipal principal);

        Task<IdentityResult> AddAsync(User user, string password);

        Task Delete(User user);

        Task UpdateStatus(User user);

        Task AddUserRole(User user);

        Task<SignInResult> Login(string userName, string password);

        Task Logout();

        Task<bool> IsUserInRole(string roleName, ClaimsPrincipal principal);
    }
}
