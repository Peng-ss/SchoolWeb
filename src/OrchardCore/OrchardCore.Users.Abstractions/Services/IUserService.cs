﻿using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace OrchardCore.Users.Services
{
    public interface IUserService
    {
        Task<IUser> CreateUserAsync(string userName, string email, string[] roleNames, string password, Action<string, string> reportError);
        Task<bool> ChangePasswordAsync(IUser user, string currentPassword, string newPassword, Action<string, string> reportError);
        Task<IUser> GetAuthenticatedUserAsync(ClaimsPrincipal principal);
    }
}