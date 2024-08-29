using ImbaBetWeb.Data;
using ImbaBetWeb.Models;
using ImbaBetWeb.Models.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Logic
{
    public class DatabaseManager
    {
        private ApplicationContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommunityManager _communityManager;
        private readonly SettingsManager _settingsManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public DatabaseManager(
            ApplicationContext context, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager,
            CommunityManager communityManager,
            SettingsManager settingsManager,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _communityManager = communityManager;
            _settingsManager = settingsManager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task DeleteAllDataAsync()
        {
            var tableNames = _context.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .ToList();

            foreach (var tableName in tableNames)
            {
                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
            }
        }

        public async Task DeleteGameDataAsync()
        {
            var tablesToBeDeleted = new[] { "Bets", "Matches", "MatchGroups", "Teams" };

            foreach (var tableName in tablesToBeDeleted)
            {
                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            await _communityManager.DeleteCommunityOfUserAsync(user);

            await _userManager.DeleteAsync(user);
        }

        public async Task DeleteProfilePicture(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.ProfilePicturePath == null)
                return;

            try
            {
                var fileToBeDeleted = _webHostEnvironment.WebRootPath + user.ProfilePicturePath;
                File.Delete(fileToBeDeleted);
                user.ProfilePicturePath = null;
                await _userManager.UpdateAsync(user);
            }
            catch { }
        }

        public async Task InitialDatabaseSeedAsync()
        {
            // Create Roles
            foreach (var role in UserRoles.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            await _settingsManager.SeedSettingsAsync();
        }

        public async Task SeedTestDataAsync()
        {
            var userList = new[]
{
                new { Email = "Ronaldo@test.de", Username = "Ronaldo", Password = "!23Qwe", Roles = new string[]{UserRoles.Admin } },
                new { Email = "Neymar@test.de", Username = "Neymar", Password = "!23Qwe", Roles = new string[]{UserRoles.Editor } },
                new { Email = "Marco@test.de", Username = "Marco", Password = "!23Qwe", Roles = Array.Empty<string>() }
            };

            foreach (var newUser in userList)
            {
                if (await _userManager.FindByEmailAsync(newUser.Email) == null)
                {
                    var user = new ApplicationUser();
                    user.Email = newUser.Email;
                    user.UserName = newUser.Username;
                    user.EmailConfirmed = true;
                    user.RemainingRenames = await _settingsManager.GetSettingAsync<int>(SettingNames.USERNAME_RENAME_LIMIT);

                    await _userManager.CreateAsync(user, newUser.Password);
                    foreach (var role in newUser.Roles)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }

                }
            }

            if (!_context.Communities.Any())
            {
                var user = await _userManager.FindByEmailAsync(userList.First().Email);
                var community = new Community()
                {
                    OwnerId = user.Id,
                    Name = "Die wilde Bande"
                };
                _context.Communities.Add(community);
                await _context.SaveChangesAsync();
                user.MemberOfCommunityId = community.Id;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateRolesAsync(string userId, bool shouldBeAdmin, bool shouldBeEditor)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return false;
            }

            var hasAdminRole = await _userManager.IsInRoleAsync(user, UserRoles.Admin);
            if (hasAdminRole && !shouldBeAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
            }
            else if(!hasAdminRole && shouldBeAdmin)
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

        public async Task<ApplicationUser> CreateAdminAsync()
        {
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            var user = new ApplicationUser();
            user.Email = _configuration.GetSection("InitialSetup")["AdminAccountEMail"];
            user.UserName = _configuration.GetSection("InitialSetup")["AdminAccountUsername"];
            user.EmailConfirmed = true;
            user.RemainingRenames = await _settingsManager.GetSettingAsync<int>(SettingNames.USERNAME_RENAME_LIMIT);

            if (await _userManager.FindByEmailAsync(user.Email) == null)
            {
                await _userManager.CreateAsync(user, _configuration.GetSection("InitialSetup")["AdminAccountPassword"]);
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return user;
        }

    }
}
