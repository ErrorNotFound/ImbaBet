using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;
using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Business
{
    public class DatabaseManager(
        IDataStoreManager dataStoreManager,
        RoleManager<IdentityRole> roleManager,
        UserManager<MyIdentityUser> userManager,
        CommunityManager communityManager,
        IWebHostEnvironment webHostEnvironment,
        IConfiguration configuration)
    {
        private readonly IDataStoreManager _dataStoreManager = dataStoreManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<MyIdentityUser> _userManager = userManager;
        private readonly CommunityManager _communityManager = communityManager;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly IConfiguration _configuration = configuration;

        public async Task DeleteMatchplanAsync()
        {      
            await _dataStoreManager.DeleteMatchplanAsync();
        }

        public async Task<Player> GetPlayerAsync(int playerId)
        {
            var players = await _dataStoreManager.GetPlayersAsync();
            var player = players.SingleOrDefault(x => x.Id == playerId);
            if (player != null)
            {
                return player;
            }
            throw new Exception($"User with id ({playerId}) not found");
        }

        public async Task DeletePlayerAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                await _communityManager.DeleteCommunityOfPlayerAsync(user.PlayerId);
                await _userManager.DeleteAsync(user);
            }
        }

        public async Task<bool> DeleteProfilePicture(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var player = await _dataStoreManager.GetPlayerByIdAsync(user.PlayerId);

            if(player.ProfilePicturePath == null)
                return true;

            try
            {
                var fileToBeDeleted = _webHostEnvironment.WebRootPath + player.ProfilePicturePath;
                File.Delete(fileToBeDeleted);
                player.ProfilePicturePath = null;
                await _dataStoreManager.UpdatePlayersAsync([player]);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task SetProfilePicture(string userId, string path)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return;

            var player = await _dataStoreManager.GetPlayerByIdAsync(user.PlayerId);

            player.ProfilePicturePath = path;
            await _dataStoreManager.UpdatePlayersAsync([player]);
            return;
        }

        public async Task<bool> ConfirmEMail(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return true;
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

            await _dataStoreManager.SeedSettingsAsync();
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
                    var player = new Player();
                    player.Id = await _dataStoreManager.CreatePlayerAsync(player);

                    var user = new MyIdentityUser();
                    user.PlayerId = player.Id;
                    user.Email = newUser.Email;
                    user.UserName = newUser.Username;
                    user.EmailConfirmed = true;
                    user.RemainingRenames = await _dataStoreManager.GetSettingValueAsync<int>(SettingNames.USERNAME_RENAME_LIMIT);

                    await _userManager.CreateAsync(user, newUser.Password);
                    foreach (var role in newUser.Roles)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }

                }
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();
            if (!communities.Any())
            {
                var idUser = await _userManager.FindByEmailAsync(userList.First().Email);
                if (idUser == null)
                    return;

                var players = await _dataStoreManager.GetPlayersAsync();
                var player = players.SingleOrDefault(u => u.Id == idUser.PlayerId);
                if (player == null)
                    return;

                var community = new Community()
                {
                    OwnerId = player.Id,
                    Name = "Die wilde Bande"
                };
                await _dataStoreManager.AddCommunityAsync(community);

                player.MemberOfCommunityId = community.Id;
                await _dataStoreManager.UpdatePlayersAsync([player]);
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

        public async Task CreateAdminAsync()
        {
            // only create admin account if there is no other admin
            var adminUsers = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
            if (adminUsers.Count > 0)
            {
                return;
            }

            var player = new Player();
            player.Id = await _dataStoreManager.CreatePlayerAsync(player);

            var user = new MyIdentityUser();
            user.PlayerId = player.Id;
            user.Email = _configuration.GetSection("InitialSetup")["AdminAccountEMail"];
            user.UserName = _configuration.GetSection("InitialSetup")["AdminAccountUsername"];
            user.EmailConfirmed = true;
            user.RemainingRenames = await _dataStoreManager.GetSettingValueAsync<int>(SettingNames.USERNAME_RENAME_LIMIT);

            if (user.Email != null && await _userManager.FindByEmailAsync(user.Email) == null)
            {
                await _userManager.CreateAsync(user, _configuration.GetSection("InitialSetup")["AdminAccountPassword"]!);
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }

            return;
        }

    }
}
