using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;
using System.ComponentModel;
using System.Reflection;

namespace ImbaBetWeb.DataAccess
{
    public class DataStoreManager(IBetStore betStore, IPlayerStore playerStore, ICommunityStore communityStore, IMatchGroupStore matchGroupStore, IMatchStore matchStore, ISettingStore settingStore, ITeamStore teamStore) : IDataStoreManager
    {
        private readonly IBetStore betStore = betStore;
        private readonly IPlayerStore playerStore = playerStore;
        private readonly ICommunityStore communityStore = communityStore;
        private readonly IMatchGroupStore matchGroupStore = matchGroupStore;
        private readonly IMatchStore matchStore = matchStore;
        private readonly ISettingStore settingStore = settingStore;
        private readonly ITeamStore teamStore = teamStore;

        private Dictionary<string, Setting> _cachedSettings = [];

        public static DataStoreManager CreateDefault(string connectionString)
        {
            return new DataStoreManager(
                new SqlBetStore(connectionString), 
                new SqlPlayerStore(connectionString), 
                new SqlCommunityStore(connectionString), 
                new SqlMatchGroupStore(connectionString), 
                new SqlMatchStore(connectionString), 
                new SqlSettingStore(connectionString), 
                new SqlTeamStore(connectionString));
        }

        public async Task Initialize()
        {
            await Task.WhenAll(
                betStore.EnsureInitializedAsync(),
                playerStore.EnsureInitializedAsync(),
                communityStore.EnsureInitializedAsync(),
                matchGroupStore.EnsureInitializedAsync(),
                matchStore.EnsureInitializedAsync(),
                settingStore.EnsureInitializedAsync(),
                teamStore.EnsureInitializedAsync());
        }

        public async Task<Matchplan> GetMatchplanAsync()
        {
            var matchGroups = await matchGroupStore.GetAllAsync();
            var matches = await matchStore.GetAllAsync();
            var teams = await teamStore.GetAllAsync();

            foreach(var match in matches)
            {
                match.TeamA = teams.SingleOrDefault(x => match.TeamATeamId == x.Id);
                match.TeamB = teams.SingleOrDefault(x => match.TeamBTeamId == x.Id);
                match.MatchGroup = matchGroups.Single(x => match.MatchGroupId == x.Id);
            }

            foreach(var matchGroup in matchGroups)
            {
                matchGroup.Matches = matches.Where(m => matchGroup.Id == m.MatchGroupId).ToList();
            }

            return new Matchplan() { Matches = matches, MatchGroups = matchGroups, Teams = teams};
        }
        public async Task ImportMatchplanAsync(Matchplan matchplan)
        {
            foreach(var team in matchplan.Teams)
            {
                team.Id = await teamStore.CreateAsync(team);
            }

            foreach (var matchGroup in matchplan.MatchGroups)
            {
                matchGroup.Id = await matchGroupStore.CreateAsync(matchGroup);
            }

            foreach(var match in matchplan.Matches)
            {
                match.MatchGroupId = match.MatchGroup!.Id; // this should have been set in the lines before
                match.TeamATeamId = match.TeamA?.Id; // optional
                match.TeamBTeamId = match.TeamB?.Id; // optional
                match.Id = await matchStore.CreateAsync(match);
            }
        }
        public async Task DeleteMatchplanAsync()
        {
            await betStore.DeleteAllAsync();
            await matchStore.DeleteAllAsync();
            await matchGroupStore.DeleteAllAsync();
            await teamStore.DeleteAllAsync();
        }

        public async Task<IEnumerable<Bet>> GetBetsAsync()
        {
            var bets = await betStore.GetAllAsync();
            var matchplan = await GetMatchplanAsync();
            var players = await playerStore.GetAllAsync();

            foreach (var bet in bets)
            {
                bet.Match = matchplan.Matches.Single(m => bet.MatchId == m.Id);
                bet.Player = players.Single(u => bet.PlayerId == u.Id);
            }

            return bets;
        }


        public async Task<IEnumerable<Bet>> GetBetsOfPlayerAsync(Player player)
        {
            var bets = await GetBetsAsync();

            return bets.Where(bet => bet.PlayerId == player.Id);
        }

        public async Task<IEnumerable<Bet>> GetBetsOfMatchAsync(Match match)
        {
            var bets = await GetBetsAsync();

            return bets.Where(bet => bet.MatchId == match.Id);
        }

        public async Task UpdateBetsAsync(IEnumerable<Bet> bets)
        {
            foreach(var bet in bets)
            {
                await betStore.UpdateAsync(bet);
            }
        }
        public async Task<int> CreatePlayerAsync(Player player)
        {
            return await playerStore.CreateAsync(player);
        }

        public async Task<Player> GetPlayerByIdAsync(int id)
        {
            var players = await GetPlayersAsync();
            return players.Single(u => u.Id == id);
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            var players = await playerStore.GetAllAsync();
            var communities = await GetCommunitiesAsync();

            foreach (var player in players.Where(u => u.MemberOfCommunityId != null))
            {
                player.Community = communities.SingleOrDefault(c => c.Id == player.MemberOfCommunityId);
            }

            return players;
        }

        public async Task UpdatePlayersAsync(IEnumerable<Player> players)
        {
            foreach (var player in players)
            {
                await playerStore.UpdateAsync(player);
            }
        }

        public async Task<IEnumerable<Community>> GetCommunitiesAsync()
        {
            var communities = await communityStore.GetAllAsync();
            var players = await playerStore.GetAllAsync();

            foreach(var player in players)
            {
                player.Community = communities.SingleOrDefault(c => c.Id == player.MemberOfCommunityId);
            }

            foreach (var community in communities)
            {
                community.Members = players.Where(u => community.Id == u.MemberOfCommunityId).ToList();
                community.Owner = players.Single(u => community.OwnerId == u.Id);
            }

            return communities;
        }

        public async Task AddCommunityAsync(Community community)
        {
            await communityStore.CreateAsync(community);
        }

        public async Task UpdateCommunitiesAsync(IEnumerable<Community> communities)
        {
            foreach (var community in communities)
            {
                await communityStore.UpdateAsync(community);
            }
        }

        public async Task DeleteCommunityAsync(int communityId)
        {
            var communities = await communityStore.GetAllAsync();
            var toBeDeleted = communities.SingleOrDefault(c => c.Id == communityId);
            if (toBeDeleted != null)
            {
                await communityStore.DeleteAsync(toBeDeleted);
            }
        }

        public async Task UpdateMatchesAsync(IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                await matchStore.UpdateAsync(match);
            }
        }

        public async Task<IEnumerable<Setting>> GetSettingsAsync()
        {
            var settings = await settingStore.GetAllAsync();
            _cachedSettings = settings.ToDictionary(k => k.Id, v => v);

            return settings;
        }

        public async Task<T> GetCachedSettingValueAsync<T>(string key) where T : IConvertible
        {
            if (_cachedSettings.TryGetValue(key, out var setting))
            {
                return (T)Convert.ChangeType(setting.Value, typeof(T));
            }

            return await GetSettingValueAsync<T>(key);
        }

        public async Task<T> GetSettingValueAsync<T>(string key) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);

            // update cache
            _cachedSettings[setting.Id] = setting;

            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }

        public async Task SetSettingValueAsync<T>(string key, T value) where T : IConvertible
        {
            var setting = await GetSettingInternal(key);
            setting.Value = (string)Convert.ChangeType(value, typeof(string));

            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Id] = setting;
        }

        public async Task ResetSettingAsync(string key)
        {
            var setting = await GetSettingInternal(key);
            setting.Value = setting.Default;

            await settingStore.UpdateAsync(setting);

            // update cache
            _cachedSettings[setting.Id] = setting;
        }

        private async Task<Setting> GetSettingInternal(string key)
        {
            return await settingStore.GetAsync(key) ?? throw new Exception($"Setting not found: {key}");
        }

        public async Task SeedSettingsAsync()
        {
            var fieldInfos = typeof(SettingNames)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();

            var settings = fieldInfos.Select(fi =>
            {
                var value = fi.GetRawConstantValue();
                if (value == null)
                    return null;
                var defaultValueAttribute = fi.GetCustomAttribute<DefaultValueAttribute>();
                var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();

                var setting = new Setting()
                {
                    Id = (string)value,
                    Default = (string)(defaultValueAttribute?.Value ?? ""),
                    Value = (string)(defaultValueAttribute?.Value ?? ""),
                    Description = descriptionAttribute?.Description ?? "",
                };

                return setting;
            }).ToList();

            // only add settings which are not null and don't exist already in db
            var availableSettings = await settingStore.GetAllAsync();
            var settingsToBeAdded = settings.Where(x => x != null).Select(x => x!).Where(s => !availableSettings.Any(x => x.Id == s.Id));

            foreach (var setting in settingsToBeAdded)
            {
                await settingStore.CreateAsync(setting);
            }
        }
    }
}
