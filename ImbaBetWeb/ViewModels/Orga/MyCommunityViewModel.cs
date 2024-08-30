using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Models;
using ImbaBetWeb.ViewModels.DTO;
using System.ComponentModel.DataAnnotations;

namespace ImbaBetWeb.ViewModels.Orga
{
    public class MyCommunityViewModel
    {
        public required List<Community> Communities { get; set; }

        public required ApplicationUser User { get; set; }
    }
}
