using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Model
{
    public class MyIdentityUser : IdentityUser
    {
        public int PlayerId { get; set; }

        public int RemainingRenames { get; set; }
    }
}
