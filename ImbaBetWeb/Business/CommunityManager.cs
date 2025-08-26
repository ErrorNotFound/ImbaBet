using ImbaBetWeb.DataAccess.Interfaces;
using ImbaBetWeb.Model;
using ImbaBetWeb.Model.Consts;

namespace ImbaBetWeb.Business
{
    public class CommunityManager(
        IDataStoreManager manager,
        SettingsManager settingsManager)
    {
        private readonly IDataStoreManager _dataStoreManager = manager;
        private readonly SettingsManager _settingsManager = settingsManager;

        public async Task<IEnumerable<Community>> GetCommunitiesAsync()
        {
            return await _dataStoreManager.GetCommunitiesAsync();
        }

        public async Task CreateCommunityAsync(BettingUser owner, string name)
        {
            if(false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_CREATE))
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

        public async Task DeleteCommunityOfUserAsync(int userId)
        {
            var users = await _dataStoreManager.GetUsersAsync();
            var user = users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                await DeleteCommunityOfUserAsync(user);
            }
        }

        public async Task DeleteCommunityOfUserAsync(BettingUser user)
        {
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var communityOfUser = communities.FirstOrDefault(com => com.OwnerId == user.Id);

            if (communityOfUser != null)
            {
                await DeleteCommunityAsync(communityOfUser.Id);
            }
        }

        public async Task DeleteCommunityAsync(int communityId)
        {
            await _dataStoreManager.DeleteCommunityAsync(communityId);
        }

        public async Task<bool> JoinCommunityAsync(BettingUser user, int communityId)
        {
            if (false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_JOIN))
            {
                return false;
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var community = communities.FirstOrDefault(x => x.Id == communityId);

            if (user != null && !user.MemberOfCommunityId.HasValue)
            {
                user.MemberOfCommunityId = communityId;
                await _dataStoreManager.UpdateUsersAsync([user]);

                return true;
            }

            return false;
        }

        public async Task<bool> LeaveCommunityAsync(BettingUser user)
        {
            if (false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_LEAVE))
            {
                return false;
            }

            var communities = await _dataStoreManager.GetCommunitiesAsync();

            if (user != null && user.MemberOfCommunityId.HasValue) 
            {
                var community = communities.Single(x => x.Id == user.MemberOfCommunityId);
                if(community.OwnerId != user.Id)// may only leave if not owner of the community
                {
                    user.MemberOfCommunityId = null;
                    await _dataStoreManager.UpdateUsersAsync([user]);
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> UpdateCommunityMembershipAsync(int userId, int? communityId)
        {
            var user = await _dataStoreManager.GetUserByIdAsync(userId);
            var communities = await _dataStoreManager.GetCommunitiesAsync();
            var community = communities.FirstOrDefault(x => x.Id == communityId);

            if(user == null)
            {
                return false;
            }

            if (user.MemberOfCommunityId == communityId) // already up to date
            {
                return true;
            }

            if (communities.Any(c => c.OwnerId == user.Id)) // may only change or remove ownership if not already owner of a community
            {
                return false;
            }

            user.MemberOfCommunityId = communityId;
            await _dataStoreManager.UpdateUsersAsync([user]);

            return true;
        }

        public async Task<bool> KickMemberAsync(int communityId, int userId)
        {
            if (false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_LEAVE))
            {
                return false;
            }

            var user = await _dataStoreManager.GetUserByIdAsync(userId);

            if (user != null 
                && user.MemberOfCommunityId == communityId)
            {
                user.MemberOfCommunityId = null;
                await _dataStoreManager.UpdateUsersAsync([user]);
                return true;
            }

            return false;
        }

        public async Task<bool> PromoteToOwnerAsync(int communityId, int userId)
        {
            var newOwner = await _dataStoreManager.GetUserByIdAsync(userId);
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
            var users = await _dataStoreManager.GetUsersAsync();

            foreach (var community in communities)
            {
                // make sure that an owner is also member of the community
                var user = users.SingleOrDefault(u => u.Id == community.OwnerId);
                if (user != null && user.MemberOfCommunityId != community.Id)
                {
                    user.MemberOfCommunityId = community.Id;
                    await _dataStoreManager.UpdateUsersAsync([user]);
                }  
            }

            await _dataStoreManager.UpdateCommunitiesAsync(communities);
        }
    }
}
