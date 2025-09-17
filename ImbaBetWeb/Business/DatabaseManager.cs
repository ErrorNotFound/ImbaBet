using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class DatabaseManager(
        IDataStoreManager _dataStoreManager,
        CommunityManager _communityManager,
        PlayerManager _playerManager,
        IConfiguration configuration,
        IdentityManager identityManager)
    {
        private readonly IDataStoreManager _dataStoreManager = _dataStoreManager;
        private readonly CommunityManager _communityManager = _communityManager;
        
        private readonly IConfiguration _configuration = configuration;
        private readonly IdentityManager _identityManager = identityManager;

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _identityManager.GetIdentityUserByIdAsync(userId);
            if (user != null)
            {
                await _communityManager.DeleteCommunityOfPlayerAsync(user.PlayerId);
                await _playerManager.DeletePlayer(user.PlayerId);
                await _identityManager.DeleteIdentityUserAsync(userId);
            }
        }

        public async Task InitialDatabaseSeedAsync()
        {
            await _identityManager.SeedUserRolesAsync();

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
                if (await _identityManager.GetIdentityUserByEMailAsync(newUser.Email) == null)
                {
                    var player = new Player();
                    player.Id = await _dataStoreManager.CreatePlayerAsync(player);

                    var user = new MyIdentityUser();
                    user.PlayerId = player.Id;
                    user.Email = newUser.Email;
                    user.UserName = newUser.Username;
                    user.EmailConfirmed = true;
                    user.RemainingRenames = await _dataStoreManager.GetSettingValueAsync<int>(SettingNames.USERNAME_RENAME_LIMIT);

                    await _identityManager.CreateIdentityUserAsync(user, newUser.Password);
                    foreach (var role in newUser.Roles)
                    {
                        await _identityManager.AddRoleToIdentityAsync(user, role);
                    }

                }
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();
            if (!communities.Any())
            {
                var idUser = await _identityManager.GetIdentityUserByEMailAsync(userList.First().Email);
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
                await _dataStoreManager.CreateCommunityAsync(community);

                player.MemberOfCommunityId = community.Id;
                await _dataStoreManager.UpdatePlayersAsync([player]);
            }
        }

        public async Task CreateAdminAsync()
        {
            // only create admin account if there is no other admin
            var adminUsers = await _identityManager.GetUsersInRoleAsync(UserRoles.Admin);
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

            if (user.Email != null && await _identityManager.GetIdentityUserByEMailAsync(user.Email) == null)
            {
                await _identityManager.CreateIdentityUserAsync(user, _configuration.GetSection("InitialSetup")["AdminAccountPassword"]!);
                await _identityManager.AddRoleToIdentityAsync(user, UserRoles.Admin);
            }

            return;
        }

    }
}
