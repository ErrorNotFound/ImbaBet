using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ImbaBetWeb.Models;


public class ApplicationUser : IdentityUser
{
    public virtual Community? OwnerOfCommunity { get; set; }

    public int? MemberOfCommunityId { get; set; }
    public virtual Community? MemberOfCommunity { get; set; }

    public virtual IList<Bet> Bets { get; set; }

    public int Points { get; set; }

    public int RemainingRenames { get; set; }

    public string? ProfilePicturePath { get; set; }
}

