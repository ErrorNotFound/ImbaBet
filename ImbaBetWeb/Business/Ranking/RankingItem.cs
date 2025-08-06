using ImbaBetWeb.Models;

namespace ImbaBetWeb.Business.Ranking
{
    public record RankingItem<T>
    {
        public int Rank { get; set; }

        public required T Details { get; set; }
        
        public int Points { get; set; }

    }
}
