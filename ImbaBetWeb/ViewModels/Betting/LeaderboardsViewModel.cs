using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class LeaderboardsViewModel
    {
        public required IList<RankingItem<UserDetails>> UserRanking { get; set; }
        public required IList<RankingItem<CommunityDetails>> CommunityRanking { get; set; }

        public required IList<RankingItem<UserDetails>>? CommunityInternalRanking { get; set; }
    }
}
