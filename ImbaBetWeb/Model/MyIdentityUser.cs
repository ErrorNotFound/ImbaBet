using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Model
{
    public class MyIdentityUser : IdentityUser
    {
        public int BettingUserId { get; set; }

        public int RemainingRenames { get; set; }
    }
}
