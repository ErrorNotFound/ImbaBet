using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;
using Microsoft.AspNetCore.Identity;
using System.Configuration;

namespace ImbaBetWeb.Business
{
    public class IdentityManager(
        RoleManager<IdentityRole> _roleManager,
        UserManager<MyIdentityUser> _userManager)
    {
        private readonly RoleManager<IdentityRole> _roleManager = _roleManager;
        private readonly UserManager<MyIdentityUser> _userManager = _userManager;

        public async Task CreateIdentityUserAsync(MyIdentityUser user, string password)
        {
            await _userManager.CreateAsync(user, password);
        }

        public MyIdentityUser GetIdentityUser(int playerId)
        {
            var identityUsers = _userManager.Users;
            var identityUser = identityUsers.SingleOrDefault(x => x.PlayerId == playerId) ?? throw new Exception($"No identity user found for playerId {playerId}");
            return identityUser;
        }

        public async Task<MyIdentityUser> GetIdentityUserByIdAsync(string userId)
        {
            var identityUser = await _userManager.FindByIdAsync(userId);
            return identityUser ?? throw new Exception($"No identity user found for user id {userId}");
        }

        public async Task<MyIdentityUser> GetIdentityUserByEMailAsync(string email)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            return identityUser ?? throw new Exception($"No identity user found for email {email}");
        }

        public async Task AddRoleToIdentityAsync(MyIdentityUser user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<IList<MyIdentityUser>> GetUsersInRoleAsync(string role)
        {
            return await _userManager.GetUsersInRoleAsync(role);
        }

        public async Task DeleteIdentityUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }
        public async Task<bool> ConfirmEMailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task SeedUserRolesAsync()
        {
            foreach (var role in UserRoles.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task<bool> UpdateRolesAsync(string userId, bool shouldBeAdmin, bool shouldBeEditor)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var hasAdminRole = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
            if (hasAdminRole && !shouldBeAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
            }
            else if (!hasAdminRole && shouldBeAdmin)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            var hasEditorRole = await _userManager.IsInRoleAsync(user, UserRoles.Editor);
            if (hasEditorRole && !shouldBeEditor)
            {
                await _userManager.RemoveFromRoleAsync(user, UserRoles.Editor);
            }
            else if (!hasEditorRole && shouldBeEditor)
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Editor);
            }
            return true;
        }
    }
}
