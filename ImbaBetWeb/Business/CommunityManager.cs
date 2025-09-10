using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class CommunityManager(
        IDataStoreManager manager)
    {
        private readonly IDataStoreManager _dataStoreManager = manager;

        public async Task<IEnumerable<Community>> GetCommunitiesAsync()
        {
            return await _dataStoreManager.GetCommunitiesAsync();
        }

        public async Task CreateCommunityAsync(Player owner, string name)
        {
            if(false == await _dataStoreManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_CREATE))
            {
                return;
            }

            var newCommunity = new Community()
            {
                OwnerId = owner.Id,
                Name = name,
                Members = [owner]
            };
            
            await _dataStoreManager.AddCommunityAsync(newCommunity);
        }

        public async Task DeleteCommunityOfPlayerAsync(int playerId)
        {
            var players = await _dataStoreManager.GetPlayersAsync();
            var player = players.SingleOrDefault(u => u.Id == playerId);
            if (player != null)
            {
                await DeleteCommunityOfPlayerAsync(player);
            }
        }

        public async Task DeleteCommunityOfPlayerAsync(Player player)
        {
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var communityOfPlayer = communities.FirstOrDefault(com => com.OwnerId == player.Id);

            if (communityOfPlayer != null)
            {
                await DeleteCommunityAsync(communityOfPlayer.Id);
            }
        }

        public async Task DeleteCommunityAsync(int communityId)
        {
            await _dataStoreManager.DeleteCommunityAsync(communityId);
        }

        public async Task<bool> JoinCommunityAsync(Player player, int communityId)
        {
            if (false == await _dataStoreManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_JOIN))
            {
                return false;
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var community = communities.FirstOrDefault(x => x.Id == communityId);

            if (player != null && !player.MemberOfCommunityId.HasValue)
            {
                player.MemberOfCommunityId = communityId;
                await _dataStoreManager.UpdatePlayersAsync([player]);

                return true;
            }

            return false;
        }

        public async Task<bool> LeaveCommunityAsync(Player player)
        {
            if (false == await _dataStoreManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_LEAVE))
            {
                return false;
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();

            if (player != null && player.MemberOfCommunityId.HasValue) 
            {
                var community = communities.Single(x => x.Id == player.MemberOfCommunityId);
                if(community.OwnerId != player.Id)// may only leave if not owner of the community
                {
                    player.MemberOfCommunityId = null;
                    await _dataStoreManager.UpdatePlayersAsync([player]);
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateCommunityMembershipAsync(int playerId, int? communityId)
        {
            var player = await _dataStoreManager.GetPlayerByIdAsync(playerId);
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var community = communities.FirstOrDefault(x => x.Id == communityId);

            if(player == null)
            {
                return false;
            }

            if (player.MemberOfCommunityId == communityId) // already up to date
            {
                return true;
            }

            if (communities.Any(c => c.OwnerId == player.Id)) // may only change or remove ownership if not already owner of a community
            {
                return false;
            }

            player.MemberOfCommunityId = communityId;
            await _dataStoreManager.UpdatePlayersAsync([player]);

            return true;
        }

        public async Task<bool> KickMemberAsync(int communityId, int playerId)
        {
            if (false == await _dataStoreManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_LEAVE))
            {
                return false;
            }

            var player = await _dataStoreManager.GetPlayerByIdAsync(playerId);

            if (player != null 
                && player.MemberOfCommunityId == communityId)
            {
                player.MemberOfCommunityId = null;
                await _dataStoreManager.UpdatePlayersAsync([player]);
                return true;
            }

            return false;
        }

        public async Task<bool> PromoteToOwnerAsync(int communityId, int playerId)
        {
            var newOwner = await _dataStoreManager.GetPlayerByIdAsync(playerId);
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var community = communities.FirstOrDefault(x => x.Id == communityId);

            if (newOwner != null
                && community != null
                && newOwner.MemberOfCommunityId == community.Id)
            {
                community.OwnerId = newOwner.Id;
                await _dataStoreManager.UpdateCommunitiesAsync([community]);
                return true;
            }

            return false;
        }

        public async Task UpdateCommunitiesAsync(IEnumerable<Community> communities)
        {
            var players = await _dataStoreManager.GetPlayersAsync();

            foreach (var community in communities)
            {
                // make sure that an owner is also member of the community
                var player = players.SingleOrDefault(u => u.Id == community.OwnerId);
                if (player != null && player.MemberOfCommunityId != community.Id)
                {
                    player.MemberOfCommunityId = community.Id;
                    await _dataStoreManager.UpdatePlayersAsync([player]);
                }  
            }

            await _dataStoreManager.UpdateCommunitiesAsync(communities);
        }
    }
}
