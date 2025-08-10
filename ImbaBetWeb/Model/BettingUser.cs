using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Model
{
    public class BettingUser : IdentityUser<int>
    {
        public int? MemberOfCommunityId { get; set; }

        public int Points { get; set; }

        public int RemainingRenames { get; set; }

        public string? ProfilePicturePath { get; set; }
    }
}
