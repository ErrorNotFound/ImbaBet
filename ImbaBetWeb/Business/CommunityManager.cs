using ImbaBetWeb.Data;
using ImbaBetWeb.Model.Consts;
using ImbaBetWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Business
{
    public class CommunityManager
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<BettingUser> _userManager;
        private readonly SettingsManager _settingsManager;

        public CommunityManager(
            ApplicationContext context, 
            UserManager<BettingUser> userManager,
            SettingsManager settingsManager)
        {
            _context = context;
            _userManager = userManager;
            _settingsManager = settingsManager;
        }
        public IQueryable<Community> Communities
        {
            get
            {
                return _context.Communities
                    .Include(x => x.Owner)
                    .Include(x => x.Members)
                    .AsQueryable();
            }
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
            _context.Communities.Add(newCommunity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCommunityOfUserAsync(BettingUser user)
        {
            var community = await Communities.FirstOrDefaultAsync(x => x.OwnerId == user.Id);
            if (community != null)
            {
                _context.Communities.Remove(community);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteCommunityAsync(int communityId)
        {
            var community = await Communities.FirstOrDefaultAsync(x => x.Id == communityId);
            if (community != null)
            {
                _context.Communities.Remove(community);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> JoinCommunityAsync(BettingUser user, int communityId)
        {
            if (false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_JOIN))
            {
                return false;
            }

            var community = await Communities.FirstOrDefaultAsync(x => x.Id == communityId);

            if (user != null
                && !user.MemberOfCommunityId.HasValue
                && user.OwnerOfCommunity == null) // may only join if not owner of a community
            {
                user.MemberOfCommunityId = communityId;
                await _context.SaveChangesAsync();
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

            if (user != null
                && user.MemberOfCommunityId.HasValue
                && user.OwnerOfCommunity == null) // may only leave if not owner of the community
            {
                user.MemberOfCommunityId = null;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateCommunityMembershipAsync(string userId, int? communityId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var community = await Communities.FirstOrDefaultAsync(x => x.Id == communityId);

            if(user == null)
            {
                return false;
            }

            if (user.MemberOfCommunityId == communityId) // already up to date
            {
                return true;
            }

            if (user.OwnerOfCommunity != null) // may only change or remove ownership if not already owner of a community
            {
                return false;
            }

            user.MemberOfCommunityId = communityId;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> KickMemberAsync(int communityId, string userId)
        {
            if (false == await _settingsManager.GetSettingValueAsync<bool>(SettingNames.ALLOW_COMMUNITY_LEAVE))
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null 
                && user.MemberOfCommunityId == communityId)
            {
                user.MemberOfCommunityId = null;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> PromoteToOwnerAsync(int communityId, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var community = await Communities.FirstOrDefaultAsync(x => x.Id == communityId);

            if (user != null
                && community != null
                && user.MemberOfCommunityId == community.Id)
            {
                community.OwnerId = user.Id;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task UpdateCommunitiesAsync(IList<Community> communities)
        {
            foreach (var community in communities)
            {
                // make sure that an owner is also member of the community
                var user = await _userManager.FindByIdAsync(community.OwnerId);
                if (user != null && user.MemberOfCommunityId != community.Id)
                {
                    user.MemberOfCommunityId = community.Id;
                }  
            }

            _context.Communities.UpdateRange(communities);
            await _context.SaveChangesAsync();
        }
    }
}
