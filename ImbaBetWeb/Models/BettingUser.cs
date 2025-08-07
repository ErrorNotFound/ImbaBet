using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Models;


public class BettingUser : IdentityUser
{
    public virtual Community? OwnerOfCommunity { get; set; }

    public int? MemberOfCommunityId { get; set; }
    public virtual Community? MemberOfCommunity { get; set; }

    public virtual IList<Bet> Bets { get; set; } = null!;

    public int Points { get; set; }

    public int RemainingRenames { get; set; }

    public string? ProfilePicturePath { get; set; }
}

