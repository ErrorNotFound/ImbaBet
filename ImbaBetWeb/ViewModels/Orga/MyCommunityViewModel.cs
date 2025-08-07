using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Orga
{
    public class MyCommunityViewModel
    {
        public required List<Community> Communities { get; set; }

        public required BettingUser User { get; set; }
    }
}
