using ImbaBetWeb.Logic.Ranking.Details;
using ImbaBetWeb.Logic.Ranking;
using ImbaBetWeb.Models;

namespace ImbaBetWeb.ViewModels.Betting
{
    public class LeaderboardsViewModel
    {
        public IList<RankingItem<UserDetails>> UserRanking { get; set; }
        public IList<RankingItem<CommunityDetails>> CommunityRanking { get; set; }

        public IList<RankingItem<UserDetails>>? CommunityInternalRanking { get; set; }
    }
}
